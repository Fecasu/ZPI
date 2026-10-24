using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebStore.DAL.EF;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public const string LocalConnection = "Server=(localdb)\\MSSQLLocalDB;Database=ZpiWebStore;Trusted_Connection=True;TrustServerCertificate=True";

    public ApplicationDbContext CreateDbContext(string[] args) => new(
        new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(
            Environment.GetEnvironmentVariable("ConnectionStrings__WebStore") ?? LocalConnection).Options);
}
