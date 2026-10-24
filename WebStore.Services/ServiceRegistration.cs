using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Services.ConcreteServices;
using WebStore.Services.Interfaces;
using WebStore.Services.Mapping;

namespace WebStore.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddWebStore(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString));
        services.AddSingleton<StoreMapper>();
        services.AddLogging();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        return services;
    }
}
