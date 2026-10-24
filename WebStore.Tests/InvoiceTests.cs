using Microsoft.EntityFrameworkCore;
using WebStore.Tests.Infrastructure;

namespace WebStore.Tests;

[Collection("SQL")]
public class InvoiceTests(SqlDatabase database) : ServiceTest(database)
{
    [Fact]
    public void Invoice_sums_saved_totals_and_update_replaces_order_set()
    {
        var p = Product(); var q = Product(15m);
        var a = Orders.AddOrUpdateOrder(OrderInput(new(p.Id, 2), new(q.Id, 1)));
        var b = Order(p.Id, 2);
        var c = Order(p.Id, 3);
        var result = Invoices.AddOrUpdateInvoice(InvoiceInput("FV/2026/1", a.Id, b.Id));
        Assert.Equal(55m, result.Amount);
        Assert.Equal(Day, result.IssueDate);
        Assert.Equal("FV/2026/1", result.Number);
        Assert.Equal(new[] { a.Id, b.Id }.Order(), result.OrderIds.Order());
        using (var db = Database.Open())
        {
            Assert.Equal(2, db.Invoices.Include(i => i.Orders).Single().Orders.Count);
            Assert.Equal(result.Id, db.Orders.Single(o => o.Id == a.Id).InvoiceId);
        }
        var price = ProductInput(p.Id); price.Price = 100;
        Products.AddOrUpdateProduct(price);
        Assert.Equal(55m, Invoices.GetInvoice(i => i.Id == result.Id)!.Amount);
        var edit = InvoiceInput("FV/2026/zmiana", b.Id, c.Id); edit.Id = result.Id; edit.IssueDate = Day.AddDays(2);
        var updated = Invoices.AddOrUpdateInvoice(edit);
        Assert.Equal(50m, updated.Amount);
        Assert.Equal(Day.AddDays(2), updated.IssueDate);
        Assert.Equal(new[] { b.Id, c.Id }.Order(), updated.OrderIds.Order());
        Assert.Null(Orders.GetOrder(o => o.Id == a.Id)!.InvoiceId);
        Assert.Equal(result.Id, Orders.GetOrder(o => o.Id == c.Id)!.InvoiceId);
        Assert.Single(Invoices.GetInvoices());
    }

    [Theory]
    [InlineData("NoNumber")][InlineData("NoDate")][InlineData("Empty")][InlineData("Null")]
    [InlineData("MixedCustomers")][InlineData("OtherInvoice")][InlineData("DuplicateNumber")]
    public void Invalid_invoice_update_preserves_data_and_all_order_associations(string error)
    {
        var p = Product(); var a = Order(p.Id); var b = Order(p.Id, 2);
        var foreignCustomer = Order(p.Id, customerId: OtherCustomer.Id);
        var otherOrder = Order(p.Id, 3);
        var invoice = Invoices.AddOrUpdateInvoice(InvoiceInput("FV/1", a.Id, b.Id));
        Invoices.AddOrUpdateInvoice(InvoiceInput("FV/2", otherOrder.Id));
        var v = InvoiceInput("Zmieniona", b.Id); v.Id = invoice.Id;
        switch (error)
        {
            case "NoNumber": v.Number = " "; break;
            case "NoDate": v.IssueDate = default; break;
            case "Empty": v.OrderIds = []; break;
            case "Null": v.OrderIds = null!; break;
            case "MixedCustomers": v.OrderIds.Add(foreignCustomer.Id); break;
            case "OtherInvoice": v.OrderIds.Add(otherOrder.Id); break;
            case "DuplicateNumber": v.Number = "FV/2"; break;
        }
        Rejected<ArgumentException>(() => Invoices.AddOrUpdateInvoice(v));
    }

    [Fact]
    public void Missing_order_rejects_entire_invoice_without_claiming_existing_order()
    {
        var p = Product(); var a = Order(p.Id);
        Rejected<KeyNotFoundException>(() => Invoices.AddOrUpdateInvoice(InvoiceInput("FV/1", a.Id, int.MaxValue)));
        Assert.Null(Orders.GetOrder(o => o.Id == a.Id)!.InvoiceId);
        Assert.Empty(Invoices.GetInvoices());
    }

    [Fact]
    public void Storage_failure_rolls_back_changes_and_same_context_can_continue()
    {
        var p = Product(); var a = Order(p.Id); var b = Order(p.Id, 2);
        var invoice = Invoices.AddOrUpdateInvoice(InvoiceInput("FV/1", a.Id, b.Id));
        // Test wymusza błąd SQL po rozpoczęciu zapisu; trigger istnieje tylko w losowej bazie testu.
        Db.Database.ExecuteSqlRaw("""
            CREATE TRIGGER RejectInvoiceUpdate ON Invoices AFTER UPDATE AS
            BEGIN THROW 51000, 'Test rollback', 1; END
            """);
        try
        {
            var edit = InvoiceInput("Błąd SQL", b.Id); edit.Id = invoice.Id;
            Rejected<DbUpdateException>(() => Invoices.AddOrUpdateInvoice(edit));
        }
        finally { Db.Database.ExecuteSqlRaw("DROP TRIGGER RejectInvoiceUpdate"); }
        var valid = InvoiceInput("FV/1", b.Id); valid.Id = invoice.Id;
        Assert.Equal(20m, Invoices.AddOrUpdateInvoice(valid).Amount);
        Assert.Null(Orders.GetOrder(o => o.Id == a.Id)!.InvoiceId);
    }
}
