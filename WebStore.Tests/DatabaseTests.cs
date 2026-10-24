using Microsoft.EntityFrameworkCore;
using WebStore.Model.DataModels;
using WebStore.Tests.Infrastructure;

namespace WebStore.Tests;

[Collection("SQL")]
public class DatabaseTests(SqlDatabase database) : ServiceTest(database)
{
    [Fact]
    public void Migration_is_applied_and_sql_schema_has_required_foreign_keys_and_tph()
    {
        using var db = Database.Open();
        Assert.Equal(db.Database.GetMigrations(), db.Database.GetAppliedMigrations());
        Assert.Empty(db.Database.GetPendingMigrations());
        Assert.False(db.Database.HasPendingModelChanges());
        var moneyColumns = db.Database.SqlQueryRaw<string>("""
            SELECT OBJECT_NAME(object_id) + '.' + name + ':'
                + CAST(precision AS varchar(2)) + ',' + CAST(scale AS varchar(2)) AS Value
            FROM sys.columns
            WHERE (object_id = OBJECT_ID('Products') AND name = 'Price')
               OR (object_id = OBJECT_ID('Orders') AND name = 'TotalAmount')
               OR (object_id = OBJECT_ID('Invoices') AND name = 'Amount')
            """).ToArray();
        Assert.Equal(new[] { "Invoices.Amount:31,2", "Orders.TotalAmount:31,2", "Products.Price:31,2" }, moneyColumns.Order());
        var expected = new[]
        {
            (typeof(Order), nameof(WebStore.Model.DataModels.Order.CustomerId), typeof(Customer)),
            (typeof(Product), nameof(WebStore.Model.DataModels.Product.SupplierId), typeof(Supplier)),
            (typeof(ProductStock), nameof(ProductStock.ProductId), typeof(Product)),
            (typeof(Address), nameof(Address.CustomerId), typeof(Customer)),
            (typeof(Product), nameof(WebStore.Model.DataModels.Product.CategoryId), typeof(Category)),
            (typeof(Order), nameof(WebStore.Model.DataModels.Order.InvoiceId), typeof(Invoice)),
            (typeof(Address), nameof(Address.StationaryStoreId), typeof(StationaryStore)),
            (typeof(StationaryStoreEmployee), nameof(StationaryStoreEmployee.StationaryStoreId), typeof(StationaryStore)),
            (typeof(OrderProduct), nameof(OrderProduct.OrderId), typeof(Order)),
            (typeof(OrderProduct), nameof(OrderProduct.ProductId), typeof(Product))
        };
        var actualConstraints = db.Database.SqlQueryRaw<string>("SELECT name AS Value FROM sys.foreign_keys").ToArray();
        foreach (var (dependent, property, principal) in expected)
        {
            var foreignKey = Assert.Single(db.Model.FindEntityType(dependent)!.GetForeignKeys(), fk => fk.Properties.Any(p => p.Name == property));
            Assert.Equal(principal, foreignKey.PrincipalEntityType.ClrType);
            Assert.False(foreignKey.IsUnique);
            Assert.Contains(foreignKey.GetConstraintName(), actualConstraints);
        }
        Assert.Equal(new[] { "OrderId", "ProductId" }, db.Model.FindEntityType(typeof(OrderProduct))!.FindPrimaryKey()!.Properties.Select(p => p.Name));
        foreach (var type in new[] { typeof(User), typeof(Customer), typeof(Supplier), typeof(StationaryStoreEmployee) })
            Assert.Equal("AspNetUsers", db.Model.FindEntityType(type)!.GetTableName());
        var tables = db.Database.SqlQueryRaw<string>("SELECT name AS Value FROM sys.tables").ToArray();
        Assert.Contains("AspNetUsers", tables);
        Assert.DoesNotContain("Customers", tables); Assert.DoesNotContain("Suppliers", tables); Assert.DoesNotContain("Employees", tables);
        var types = db.Database.SqlQueryRaw<string>("SELECT UserType AS Value FROM AspNetUsers").ToArray();
        Assert.Contains("Customer", types); Assert.Contains("Supplier", types); Assert.Contains("Employee", types);
    }

    [Fact]
    public void New_context_restores_identity_subtypes_common_fields_and_employee_store()
    {
        using var db = Database.Open();
        var customer = Assert.IsType<Customer>(db.Users.Single(x => x.Id == Customer.Id));
        var supplier = Assert.IsType<Supplier>(db.Users.Single(x => x.Id == Supplier.Id));
        var employee = Assert.IsType<StationaryStoreEmployee>(db.Users.Single(x => x.Id == Employee.Id));
        Assert.Equal("Anna", customer.FirstName); Assert.Equal("Testowa", customer.LastName);
        Assert.Equal(Day, customer.RegistrationDate);
        Assert.Equal("Dostawca", supplier.FirstName); Assert.Equal(Day, supplier.RegistrationDate);
        Assert.Equal("Ewa", employee.FirstName); Assert.Equal("Testowa", employee.LastName);
        Assert.Equal(Day, employee.RegistrationDate); Assert.Equal(Store.Id, employee.StationaryStoreId);
    }

    [Fact]
    public void Database_rejects_negative_stock_and_accepts_zero()
    {
        var p = Product();
        using var db = Database.Open();
        db.ProductStocks.Add(new ProductStock { ProductId = p.Id, Quantity = -1 });
        Assert.Throws<DbUpdateException>(() => db.SaveChanges());
        db.ChangeTracker.Clear();
        Assert.Empty(db.ProductStocks);
        db.ProductStocks.Add(new ProductStock { ProductId = p.Id, Quantity = 0 });
        db.SaveChanges();
        Assert.Equal(0, Products.GetProduct(x => x.Id == p.Id)!.Quantity);
    }

    [Fact]
    public void Database_rejects_address_without_owner_employee_without_store_and_empty_category()
    {
        using var db = Database.Open();
        db.Add(new Address { Country = "PL", City = "Kraków", PostalCode = "30-001", BuildingNumber = "1" });
        Assert.Throws<DbUpdateException>(() => db.SaveChanges()); db.ChangeTracker.Clear();
        db.Add(new StationaryStoreEmployee { FirstName = "Test", LastName = "Test", RegistrationDate = Day });
        Assert.Throws<DbUpdateException>(() => db.SaveChanges()); db.ChangeTracker.Clear();
        db.Add(new Category { Name = " ", Tag = "" });
        Assert.Throws<DbUpdateException>(() => db.SaveChanges()); db.ChangeTracker.Clear();
        Assert.Empty(db.Addresses);
        Assert.Single(db.Employees); Assert.Single(db.Categories);
    }
}
