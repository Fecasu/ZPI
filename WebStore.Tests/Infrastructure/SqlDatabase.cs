using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.EF;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace WebStore.Tests.Infrastructure;

[CollectionDefinition("SQL")]
public class SqlCollection : ICollectionFixture<SqlDatabase>;

public sealed class SqlDatabase : IDisposable
{
    private readonly string databaseName = "ZpiWebStoreTests_" + Guid.NewGuid().ToString("N");
    public string ConnectionString { get; }

    public SqlDatabase()
    {
        var connection = new SqlConnectionStringBuilder(ApplicationDbContextFactory.LocalConnection)
        { InitialCatalog = databaseName };
        ConnectionString = connection.ConnectionString;
        using var db = Open();
        db.Database.Migrate();
    }

    public ApplicationDbContext Open() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer(ConnectionString).Options);

    public void Reset()
    {
        using var db = Open();
        // Wyłącznie losowa baza utworzona przez tę instancję fixture.
        CheckDatabase();
        db.Database.ExecuteSqlRaw("""
            UPDATE AspNetUsers SET BillingAddressId = NULL, ShippingAddressId = NULL;
            DELETE FROM OrderProducts;
            DELETE FROM Orders;
            DELETE FROM Invoices;
            DELETE FROM ProductStocks;
            DELETE FROM Products;
            DELETE FROM Addresses;
            DELETE FROM AspNetUsers;
            DELETE FROM Stores;
            DELETE FROM Categories;
            """);
    }

    public void Dispose()
    {
        CheckDatabase();
        using var db = Open();
        db.Database.EnsureDeleted();
    }

    private void CheckDatabase()
    {
        if (new SqlConnectionStringBuilder(ConnectionString).InitialCatalog != databaseName
            || !databaseName.StartsWith("ZpiWebStoreTests_", StringComparison.Ordinal))
            throw new InvalidOperationException("Odmowa czyszczenia bazy spoza zestawu testów.");
    }
}
