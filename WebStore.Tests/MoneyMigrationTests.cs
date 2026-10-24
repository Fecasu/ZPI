using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Tests.Infrastructure;

namespace WebStore.Tests;

[Collection("SQL")]
public class MoneyMigrationTests
{
    private const string InitialMigration = "20261024122406_InitialCreate";

    [Fact]
    public void Upgrade_preserves_existing_prices_amounts_and_relations()
    {
        using var database = new SqlDatabase();
        using (var db = database.Open())
        {
            db.GetService<IMigrator>().Migrate(InitialMigration);
            Seed(db);
            db.Database.Migrate();
        }
        using var read = database.Open();
        Assert.Equal(10.01m, read.Products.Single().Price);
        Assert.Equal(20.02m, read.Orders.Single().TotalAmount);
        Assert.Equal(20.02m, read.Invoices.Single().Amount);
        var line = read.OrderProducts.Include(x => x.Product).Include(x => x.Order).Single();
        Assert.Equal(2, line.Quantity);
        Assert.Equal(read.Products.Single().Id, line.ProductId);
        Assert.Equal(read.Invoices.Single().Id, line.Order.InvoiceId);
        Assert.All(Scales(read), scale => Assert.Equal(2, scale));
        Assert.Empty(read.Database.GetPendingMigrations());
    }

    [Theory]
    [InlineData("Product")][InlineData("Order")][InlineData("Invoice")]
    public void Upgrade_rejects_old_values_with_excessive_precision_without_rounding_or_schema_change(string kind)
    {
        using var database = new SqlDatabase();
        using (var db = database.Open())
        {
            db.GetService<IMigrator>().Migrate(InitialMigration);
            Seed(db);
            var sql = kind switch
            {
                "Product" => "UPDATE Products SET Price = 1.234",
                "Order" => "UPDATE Orders SET TotalAmount = 1.234",
                "Invoice" => "UPDATE Invoices SET Amount = 1.234",
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };
            db.Database.ExecuteSqlRaw(sql);
            var error = Assert.Throws<SqlException>(() => db.Database.Migrate());
            Assert.Equal(51001, error.Number);
        }
        using var read = database.Open();
        Assert.Equal(new[] { InitialMigration }, read.Database.GetAppliedMigrations());
        Assert.All(Scales(read), scale => Assert.Equal(8, scale));
        Assert.Equal(kind == "Product" ? 1.234m : 10.01m, read.Products.Single().Price);
        Assert.Equal(kind == "Order" ? 1.234m : 20.02m, read.Orders.Single().TotalAmount);
        Assert.Equal(kind == "Invoice" ? 1.234m : 20.02m, read.Invoices.Single().Amount);
        Assert.Single(read.OrderProducts);
        Assert.Equal(read.Invoices.Single().Id, read.Orders.Single().InvoiceId);
    }

    private static int[] Scales(ApplicationDbContext db) => db.Database.SqlQueryRaw<int>("""
        SELECT CAST(scale AS int) AS Value FROM sys.columns
        WHERE (object_id = OBJECT_ID('Products') AND name = 'Price')
           OR (object_id = OBJECT_ID('Orders') AND name = 'TotalAmount')
           OR (object_id = OBJECT_ID('Invoices') AND name = 'Amount')
        """).ToArray();

    private static void Seed(ApplicationDbContext db)
    {
        var day = new DateTime(2026, 10, 24);
        var product = new Product
        {
            Name = "Kawa", Description = "Dane migracji", ImageBytes = [1], Price = 10.01m, Weight = 0.25f,
            Category = new Category { Name = "Kawa", Tag = "kawa" },
            Supplier = new Supplier { FirstName = "Dostawca", LastName = "Testowy", RegistrationDate = day }
        };
        var order = new Order
        {
            Customer = new Customer { FirstName = "Klient", LastName = "Testowy", RegistrationDate = day },
            OrderDate = day, TotalAmount = 20.02m,
            OrderProducts = [new OrderProduct { Product = product, Quantity = 2 }]
        };
        db.Invoices.Add(new Invoice { Number = "FV/migracja", IssueDate = day, Amount = 20.02m, Orders = [order] });
        db.SaveChanges();
        db.ChangeTracker.Clear();
    }
}
