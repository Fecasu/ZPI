using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebStore.Model.DataModels;

namespace WebStore.DAL.EF;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StationaryStoreEmployee> Employees => Set<StationaryStoreEmployee>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<StationaryStore> Stores => Set<StationaryStore>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<User>().HasDiscriminator<string>("UserType")
            .HasValue<User>("User").HasValue<Customer>("Customer")
            .HasValue<Supplier>("Supplier").HasValue<StationaryStoreEmployee>("Employee");
        b.Entity<User>().ToTable("AspNetUsers", t => t.HasCheckConstraint("CK_Employee_Store",
            "[UserType] <> 'Employee' OR [StationaryStoreId] IS NOT NULL"));
        b.Entity<Customer>().HasMany(x => x.Orders).WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Supplier>().HasMany(x => x.Products).WithOne(x => x.Supplier)
            .HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Product>().HasMany(x => x.Stocks).WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Category>().HasMany(x => x.Products).WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Customer>().HasMany(x => x.Addresses).WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<StationaryStore>().HasMany(x => x.Addresses).WithOne(x => x.StationaryStore)
            .HasForeignKey(x => x.StationaryStoreId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<StationaryStore>().HasMany(x => x.Employees).WithOne(x => x.StationaryStore)
            .HasForeignKey(x => x.StationaryStoreId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Customer>().HasOne(x => x.BillingAddress).WithMany()
            .HasForeignKey(x => x.BillingAddressId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Customer>().HasOne(x => x.ShippingAddress).WithMany()
            .HasForeignKey(x => x.ShippingAddressId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Invoice>().HasMany(x => x.Orders).WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<OrderProduct>().HasKey(x => new { x.OrderId, x.ProductId });
        b.Entity<OrderProduct>().HasOne(x => x.Order).WithMany(x => x.OrderProducts)
            .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<OrderProduct>().HasOne(x => x.Product).WithMany(x => x.OrderProducts)
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        b.Entity<Product>().Property(x => x.Price).HasPrecision(31, 2);
        b.Entity<Order>().Property(x => x.TotalAmount).HasPrecision(31, 2);
        b.Entity<Invoice>().Property(x => x.Amount).HasPrecision(31, 2);
        b.Entity<Address>().ToTable(t => t.HasCheckConstraint("CK_Address_OneOwner",
            "([CustomerId] IS NOT NULL AND [StationaryStoreId] IS NULL) OR ([CustomerId] IS NULL AND [StationaryStoreId] IS NOT NULL)"));
        b.Entity<ProductStock>().ToTable(t => t.HasCheckConstraint("CK_Stock_Quantity", "[Quantity] >= 0"));
        b.Entity<OrderProduct>().ToTable(t => t.HasCheckConstraint("CK_OrderProduct_Quantity", "[Quantity] > 0"));
        b.Entity<Product>().ToTable(t => t.HasCheckConstraint("CK_Product_PriceWeight", "[Price] > 0 AND [Weight] > 0"));
        b.Entity<Category>().ToTable(t => t.HasCheckConstraint("CK_Category_Required", "LEN(LTRIM(RTRIM([Name]))) > 0 AND LEN(LTRIM(RTRIM([Tag]))) > 0"));
        // Skrót umożliwia indeks unikalny bez limitu długości numeru faktury.
        b.Entity<Invoice>().Property<byte[]>("NumberHash").HasMaxLength(32)
            .HasComputedColumnSql("HASHBYTES('SHA2_256', CONVERT(varbinary(max), [Number]))", stored: true);
        b.Entity<Invoice>().HasIndex("NumberHash").IsUnique().HasFilter(null);
    }
}
