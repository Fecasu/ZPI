using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Tests.Infrastructure;

public abstract class ServiceTest : IDisposable
{
    protected SqlDatabase Database { get; }
    private readonly ServiceProvider provider;
    private readonly IServiceScope scope;
    protected ApplicationDbContext Db { get; }
    protected IProductService Products { get; }
    protected IAddressService Addresses { get; }
    protected IStoreService Stores { get; }
    protected IOrderService Orders { get; }
    protected IInvoiceService Invoices { get; }
    protected ErrorLog Log { get; } = new();
    protected Customer Customer { get; }
    protected Customer OtherCustomer { get; }
    protected Supplier Supplier { get; }
    protected StationaryStore Store { get; }
    protected StationaryStoreEmployee Employee { get; }
    protected Category Category { get; }
    protected static DateTime Day => new(2026, 10, 24);

    protected ServiceTest(SqlDatabase database)
    {
        Database = database;
        Database.Reset();
        provider = new ServiceCollection().AddWebStore(database.ConnectionString)
            .AddLogging(b => b.AddProvider(Log)).BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        scope = provider.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Products = scope.ServiceProvider.GetRequiredService<IProductService>();
        Addresses = scope.ServiceProvider.GetRequiredService<IAddressService>();
        Stores = scope.ServiceProvider.GetRequiredService<IStoreService>();
        Orders = scope.ServiceProvider.GetRequiredService<IOrderService>();
        Invoices = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
        Customer = new Customer { FirstName = "Anna", LastName = "Testowa", RegistrationDate = Day, UserName = "customer" };
        OtherCustomer = new Customer { FirstName = "Jan", LastName = "Testowy", RegistrationDate = Day, UserName = "other" };
        Supplier = new Supplier { FirstName = "Dostawca", LastName = "Testowy", RegistrationDate = Day, UserName = "supplier" };
        Store = new StationaryStore { Name = "Kraków" };
        Employee = new StationaryStoreEmployee { FirstName = "Ewa", LastName = "Testowa", RegistrationDate = Day, UserName = "employee", StationaryStore = Store };
        Category = new Category { Name = "Kawa", Tag = "kawa" };
        Db.AddRange(Customer, OtherCustomer, Supplier, Store, Employee, Category);
        Db.SaveChanges();
        Db.ChangeTracker.Clear();
    }

    protected AddOrUpdateProductVm ProductInput(int? id = null, string name = "Kawa") => new()
    {
        Id = id, Name = name, Description = "Opis produktu", ImageBytes = [1, 2, 3],
        Price = 10m, Weight = 0.25f, CategoryId = Category.Id, SupplierId = Supplier.Id
    };
    protected AddOrUpdateAddressVm AddressInput(int? id = null, string city = "Warszawa") => new()
    {
        Id = id, Country = "Polska", PostalCode = "00-001", City = city,
        BuildingNumber = "1A", CustomerId = Customer.Id
    };
    protected AddOrUpdateOrderVm OrderInput(params OrderLineVm[] lines) => new()
    { CustomerId = Customer.Id, OrderDate = Day, Products = lines.ToList() };
    protected AddOrUpdateInvoiceVm InvoiceInput(string number, params int[] orders) => new()
    { Number = number, IssueDate = Day, OrderIds = orders.ToList() };

    protected ProductVm Product(decimal price = 10m, string name = "Kawa")
    {
        var input = ProductInput(name: name);
        input.Price = price;
        return Products.AddOrUpdateProduct(input);
    }
    protected OrderVm Order(int productId, int quantity = 1, int? customerId = null)
    {
        var input = OrderInput(new OrderLineVm(productId, quantity));
        input.CustomerId = customerId ?? Customer.Id;
        return Orders.AddOrUpdateOrder(input);
    }

    // Odczyt nowym kontekstem obejmuje wartości i relacje, a nie tylko liczby rekordów.
    protected string Snapshot()
    {
        using var db = Database.Open();
        return JsonSerializer.Serialize(new
        {
            Users = db.Users.OrderBy(x => x.Id).Select(x => new { x.Id, x.FirstName, x.LastName, x.RegistrationDate }).ToArray(),
            Customers = db.Customers.OrderBy(x => x.Id).Select(x => new { x.Id, x.BillingAddressId, x.ShippingAddressId }).ToArray(),
            Employees = db.Employees.OrderBy(x => x.Id).Select(x => new { x.Id, x.StationaryStoreId }).ToArray(),
            Stores = db.Stores.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name }).ToArray(),
            Products = db.Products.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Description, x.ImageBytes, x.Price, x.Weight, x.CategoryId, x.SupplierId }).ToArray(),
            Stocks = db.ProductStocks.OrderBy(x => x.Id).Select(x => new { x.Id, x.ProductId, x.Quantity }).ToArray(),
            Addresses = db.Addresses.OrderBy(x => x.Id).Select(x => new { x.Id, x.Country, x.PostalCode, x.City, x.Street, x.BuildingNumber, x.ApartmentNumber, x.CustomerId, x.StationaryStoreId }).ToArray(),
            Orders = db.Orders.OrderBy(x => x.Id).Select(x => new { x.Id, x.CustomerId, x.OrderDate, x.DeliveryDate, x.TrackingNumber, x.TotalAmount, x.InvoiceId }).ToArray(),
            Lines = db.OrderProducts.OrderBy(x => x.OrderId).ThenBy(x => x.ProductId).Select(x => new { x.OrderId, x.ProductId, x.Quantity }).ToArray(),
            Invoices = db.Invoices.OrderBy(x => x.Id).Select(x => new { x.Id, x.Number, x.IssueDate, x.Amount }).ToArray()
        });
    }

    protected void Rejected<T>(Action action) where T : Exception
    {
        var before = Snapshot();
        var logs = Log.Errors.Count;
        Assert.Throws<T>(action);
        Assert.Equal(before, Snapshot());
        Assert.False(Db.ChangeTracker.HasChanges());
        Db.SaveChanges();
        Assert.Equal(before, Snapshot());
        Assert.True(Log.Errors.Count > logs);
    }

    public void Dispose() { scope.Dispose(); provider.Dispose(); }
}

public sealed class ErrorLog : ILoggerProvider
{
    public List<Exception> Errors { get; } = [];
    public ILogger CreateLogger(string categoryName) => new Capture(this);
    public void Dispose() { }
    private sealed class Capture(ErrorLog owner) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel >= LogLevel.Error && exception is not null) owner.Errors.Add(exception);
        }
    }
}
