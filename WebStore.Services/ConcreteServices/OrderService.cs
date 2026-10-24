using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Services.Mapping;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class OrderService(ApplicationDbContext context, StoreMapper mapper, ILogger<OrderService> logger)
    : BaseService(context, mapper, logger), IOrderService
{
    private IQueryable<Order> Query => Context.Orders.Include(o => o.OrderProducts);

    public OrderVm AddOrUpdateOrder(AddOrUpdateOrderVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidId(model.Id);
        var order = model.Id is > 0
            ? Query.SingleOrDefault(o => o.Id == model.Id) ?? throw new KeyNotFoundException("Nie znaleziono zamówienia.")
            : new Order();
        if (order.InvoiceId is not null) throw new ArgumentException("Zamówienie na fakturze nie podlega edycji.");
        if (model.OrderDate == default || model.DeliveryDate < model.OrderDate)
            throw new ArgumentException("Nieprawidłowa data zamówienia lub dostawy.");
        RequiredUser<Customer>(model.CustomerId);
        if (model.Products is null || model.Products.Count == 0 || model.Products.Any(p => p is null || p.Quantity <= 0)
            || model.Products.Select(p => p.ProductId).Distinct().Count() != model.Products.Count)
            throw new ArgumentException("Wymagane różne produkty z dodatnimi ilościami.");
        var ids = model.Products.Select(p => p.ProductId).ToArray();
        var products = Context.Products.Where(p => ids.Contains(p.Id)).ToDictionary(p => p.Id);
        if (products.Count != ids.Length) throw new KeyNotFoundException("Nie znaleziono produktu zamówienia.");
        var total = Pln.Total(model.Products.Select(p => (products[p.ProductId].Price, p.Quantity)));
        Mapper.Apply(model, order);
        order.TotalAmount = total;
        foreach (var removed in order.OrderProducts.Where(p => !ids.Contains(p.ProductId)).ToArray())
        {
            Context.OrderProducts.Remove(removed);
            order.OrderProducts.Remove(removed);
        }
        foreach (var line in model.Products)
        {
            var existing = order.OrderProducts.SingleOrDefault(p => p.ProductId == line.ProductId);
            if (existing is not null) existing.Quantity = line.Quantity;
            else order.OrderProducts.Add(new OrderProduct { ProductId = line.ProductId, Quantity = line.Quantity });
        }
        if (model.Id is null or 0) Context.Orders.Add(order);
        Context.SaveChanges();
        return Mapper.Map(order);
    });

    public OrderVm? GetOrder(Expression<Func<Order, bool>> predicate) => One(Query, predicate, Mapper.Map);
    public IReadOnlyList<OrderVm> GetOrders(Expression<Func<Order, bool>>? predicate = null) => Many(Query, predicate, Mapper.Map);
}
