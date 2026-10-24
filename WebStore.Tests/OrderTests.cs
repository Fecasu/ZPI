using Microsoft.EntityFrameworkCore;
using WebStore.Model.DataModels;
using WebStore.Tests.Infrastructure;
using WebStore.ViewModels.VM;

namespace WebStore.Tests;

[Collection("SQL")]
public class OrderTests(SqlDatabase database) : ServiceTest(database)
{
    [Fact]
    public void Totals_use_prices_at_save_edit_recalculates_and_stock_does_not_change()
    {
        var p = Product(); var q = Product(15m, "Herbata");
        Db.ProductStocks.Add(new ProductStock { ProductId = p.Id, Quantity = 7 });
        Db.SaveChanges(); Db.ChangeTracker.Clear();
        var v = OrderInput(new(p.Id, 2), new(q.Id, 1));
        var first = Orders.AddOrUpdateOrder(v);
        Assert.Equal(35m, first.TotalAmount);
        Assert.Null(first.DeliveryDate); Assert.Null(first.TrackingNumber); Assert.Null(first.InvoiceId);
        Assert.Equal(2, first.Products.Count);
        Assert.Equal(2, first.Products.Single(x => x.ProductId == p.Id).Quantity);
        var price = ProductInput(p.Id); price.Price = 20m;
        Products.AddOrUpdateProduct(price);
        Assert.Equal(35m, Orders.GetOrder(o => o.Id == first.Id)!.TotalAmount);
        v.Id = first.Id; v.DeliveryDate = Day.AddDays(1); v.TrackingNumber = 123456789L;
        var changed = Orders.AddOrUpdateOrder(v);
        Assert.Equal(55m, changed.TotalAmount);
        Assert.Equal(Day.AddDays(1), changed.DeliveryDate); Assert.Equal(123456789L, changed.TrackingNumber);
        Assert.Equal(7, Products.GetProduct(x => x.Id == p.Id)!.Quantity);
        // Usunięcie i dodanie pozycji oraz zmiana ilości odtwarzają końcowy zestaw.
        v.Products = [new(q.Id, 2)];
        var single = Orders.AddOrUpdateOrder(v);
        Assert.Single(single.Products); Assert.Equal(q.Id, single.Products[0].ProductId);
        Assert.Equal(30m, single.TotalAmount);
        v.Products.Add(new(p.Id, 3));
        Assert.Equal(90m, Orders.AddOrUpdateOrder(v).TotalAmount);
        using var db = Database.Open();
        var persisted = db.Orders.Include(o => o.OrderProducts).Single();
        Assert.Equal(2, persisted.OrderProducts.Count);
        Assert.Equal(7, db.ProductStocks.Single().Quantity);
    }

    [Fact]
    public void Product_can_appear_in_multiple_orders_of_the_same_customer()
    {
        var p = Product(); var q = Product(15m);
        var first = Orders.AddOrUpdateOrder(OrderInput(new(p.Id, 2), new(q.Id, 1)));
        var second = Order(p.Id);
        using var db = Database.Open();
        Assert.Equal(2, db.Products.Include(p => p.OrderProducts).Single(x => x.Id == p.Id).OrderProducts.Count);
        var customer = db.Customers.Include(c => c.Orders).Single(c => c.Id == Customer.Id);
        Assert.Equal(new[] { first.Id, second.Id }.Order(), customer.Orders.Select(o => o.Id).Order());
    }

    [Theory]
    [InlineData("Empty")][InlineData("Null")][InlineData("NullLine")][InlineData("Zero")][InlineData("Negative")]
    [InlineData("Duplicate")][InlineData("NoDate")][InlineData("DeliveryBeforeOrder")][InlineData("WrongCustomerType")]
    public void Invalid_order_cannot_change_existing_data(string error)
    {
        var p = Product(); var order = Order(p.Id);
        var v = OrderInput(new OrderLineVm(p.Id, 2)); v.Id = order.Id; v.TrackingNumber = 99;
        switch (error)
        {
            case "Empty": v.Products = []; break;
            case "Null": v.Products = null!; break;
            case "NullLine": v.Products = [null!]; break;
            case "Zero": v.Products = [new(p.Id, 0)]; break;
            case "Negative": v.Products = [new(p.Id, -1)]; break;
            case "Duplicate": v.Products.Add(new(p.Id, 1)); break;
            case "NoDate": v.OrderDate = default; break;
            case "DeliveryBeforeOrder": v.DeliveryDate = Day.AddDays(-1); break;
            case "WrongCustomerType": v.CustomerId = Supplier.Id; break;
        }
        Rejected<ArgumentException>(() => Orders.AddOrUpdateOrder(v));
    }

    [Theory]
    [InlineData(true)][InlineData(false)]
    public void Missing_customer_or_product_does_not_create_partial_order(bool customer)
    {
        var p = Product();
        var v = OrderInput(new(p.Id, 2), new(int.MaxValue, 1));
        if (customer) { v.CustomerId = int.MaxValue; v.Products = [new(p.Id, 1)]; }
        Rejected<KeyNotFoundException>(() => Orders.AddOrUpdateOrder(v));
    }

    [Fact]
    public void Invoiced_order_can_only_be_edited_after_release_from_invoice()
    {
        var p = Product(); var a = Order(p.Id); var b = Order(p.Id, 2);
        var invoice = Invoices.AddOrUpdateInvoice(InvoiceInput("FV/1", a.Id, b.Id));
        var edit = OrderInput(new OrderLineVm(p.Id, 5)); edit.Id = a.Id;
        Rejected<ArgumentException>(() => Orders.AddOrUpdateOrder(edit));
        var release = InvoiceInput("FV/1", b.Id); release.Id = invoice.Id;
        Invoices.AddOrUpdateInvoice(release);
        Assert.Null(Orders.GetOrder(o => o.Id == a.Id)!.InvoiceId);
        Assert.Equal(50m, Orders.AddOrUpdateOrder(edit).TotalAmount);
        Assert.Equal(20m, Invoices.GetInvoice(i => i.Id == invoice.Id)!.Amount);
    }
}
