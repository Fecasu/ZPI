using System.Linq.Expressions;
using WebStore.Model.DataModels;
using WebStore.Tests.Infrastructure;
using WebStore.ViewModels.VM;

namespace WebStore.Tests;

// WU-01–07 dla każdej z pięciu usług, przez publiczne interfejsy i rzeczywisty SQL.
[Collection("SQL")]
public class ServiceContractTests(SqlDatabase database) : ServiceTest(database)
{
    public static TheoryData<string> Services => new() { "Product", "Address", "Store", "Order", "Invoice" };

    [Theory, MemberData(nameof(Services))]
    public void Add_null_or_zero_id_update_and_filtered_reads(string kind)
    {
        var c = Contract(kind);
        var baseline = c.List(null).ToArray();
        var first = c.Save(null, "Pierwszy", false);
        var second = c.Save(0, "Drugi", false);
        Assert.NotEqual(first, second);
        Assert.Equal(baseline.Append(first).Append(second).Order(), c.List(null).Order());
        Assert.Equal(new[] { first }, c.List([first]));
        Assert.Equal(new[] { first, second }.Order(), c.List([first, second]).Order());
        Assert.Contains(c.One([first, second])!.Value, new[] { first, second });
        Assert.Null(c.One([-1]));
        Assert.Empty(c.List([-1]));
        Assert.Equal(first, c.Save(first, "Zmieniony", false));
        Assert.Equal("Zmieniony", c.Label(first));
        Assert.Equal(baseline.Length + 2, c.List(null).Count);
    }

    [Theory, MemberData(nameof(Services))]
    public void Empty_database_returns_null_and_empty_list(string kind)
    {
        Database.Reset();
        var c = Contract(kind, prepare: false);
        Assert.Empty(c.List(null));
        Assert.Null(c.One([1]));
    }

    [Theory, MemberData(nameof(Services))]
    public void Null_input_and_required_predicate_are_logged_and_rejected(string kind)
    {
        var c = Contract(kind);
        Rejected<ArgumentNullException>(c.NullInput);
        Rejected<ArgumentNullException>(() => c.One(null));
    }

    [Theory, MemberData(nameof(Services))]
    public void Invalid_ids_and_business_data_do_not_write_or_poison_context(string kind)
    {
        var c = Contract(kind);
        var id = c.Save(null, "Oryginał", false);
        Rejected<ArgumentException>(() => c.Save(-1, "Błąd", false));
        Rejected<KeyNotFoundException>(() => c.Save(int.MaxValue, "Błąd", false));
        Rejected<ArgumentException>(() => c.Save(null, "Błąd", true));
        Rejected<ArgumentException>(() => c.Save(id, "Błąd", true));
        Assert.Equal("Oryginał", c.Label(id));
        Assert.Equal(id, c.Save(id, "Poprawiony", false));
        Assert.Equal("Poprawiony", c.Label(id));
    }

    private Adapter Contract(string kind, bool prepare = true)
    {
        var productId = prepare && kind is "Order" or "Invoice" ? Product().Id : 0;
        var ordersByName = new Dictionary<string, int>();
        int OrderId(string name)
        {
            if (!ordersByName.TryGetValue(name, out var value)) ordersByName[name] = value = Order(productId).Id;
            return value;
        }
        return kind switch
        {
            "Product" => new(
                (id, name, invalid) => { var v = ProductInput(id, invalid ? " " : name); return Products.AddOrUpdateProduct(v).Id; },
                () => Products.AddOrUpdateProduct(null!),
                ids => Products.GetProduct(ids is null ? null! : p => ids.Contains(p.Id))?.Id,
                ids => Products.GetProducts(ids is null ? null : p => ids.Contains(p.Id)).Select(p => p.Id).ToArray(),
                id => Products.GetProduct(p => p.Id == id)!.Name),
            "Address" => new(
                (id, name, invalid) => Addresses.AddOrUpdateAddress(AddressInput(id, invalid ? " " : name)).Id,
                () => Addresses.AddOrUpdateAddress(null!),
                ids => Addresses.GetAddress(ids is null ? null! : a => ids.Contains(a.Id))?.Id,
                ids => Addresses.GetAddresses(ids is null ? null : a => ids.Contains(a.Id)).Select(a => a.Id).ToArray(),
                id => Addresses.GetAddress(a => a.Id == id)!.City),
            "Store" => new(
                (id, name, invalid) => Stores.AddOrUpdateStore(new() { Id = id, Name = invalid ? " " : name }).Id,
                () => Stores.AddOrUpdateStore(null!),
                ids => Stores.GetStore(ids is null ? null! : s => ids.Contains(s.Id))?.Id,
                ids => Stores.GetStores(ids is null ? null : s => ids.Contains(s.Id)).Select(s => s.Id).ToArray(),
                id => Stores.GetStore(s => s.Id == id)!.Name),
            "Order" => new(
                (id, name, invalid) =>
                {
                    var v = OrderInput(new OrderLineVm(productId, invalid ? 0 : 1));
                    v.Id = id;
                    v.TrackingNumber = NameNumber(name);
                    return Orders.AddOrUpdateOrder(v).Id;
                },
                () => Orders.AddOrUpdateOrder(null!),
                ids => Orders.GetOrder(ids is null ? null! : o => ids.Contains(o.Id))?.Id,
                ids => Orders.GetOrders(ids is null ? null : o => ids.Contains(o.Id)).Select(o => o.Id).ToArray(),
                id => NumberName(Orders.GetOrder(o => o.Id == id)!.TrackingNumber)),
            "Invoice" => new(
                (id, name, invalid) =>
                {
                    // Dane pomocnicze powstają przed próbą, żeby odrzucona operacja nie zmieniała fixture.
                    var existing = id is > 0 and < int.MaxValue ? Invoices.GetInvoice(i => i.Id == id) : null;
                    var orderId = existing?.OrderIds[0] ?? OrderId("wspólne");
                    var v = InvoiceInput(invalid ? " " : name, orderId);
                    if (id is null or 0 && !invalid && Invoices.GetInvoices().Count > 0)
                        v.OrderIds = [OrderId(name)];
                    v.Id = id;
                    return Invoices.AddOrUpdateInvoice(v).Id;
                },
                () => Invoices.AddOrUpdateInvoice(null!),
                ids => Invoices.GetInvoice(ids is null ? null! : i => ids.Contains(i.Id))?.Id,
                ids => Invoices.GetInvoices(ids is null ? null : i => ids.Contains(i.Id)).Select(i => i.Id).ToArray(),
                id => Invoices.GetInvoice(i => i.Id == id)!.Number),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    private static long NameNumber(string name) => name switch { "Pierwszy" => 1, "Drugi" => 2, "Zmieniony" => 3, "Oryginał" => 4, "Poprawiony" => 5, _ => 6 };
    private static string NumberName(long? value) => value switch { 1 => "Pierwszy", 2 => "Drugi", 3 => "Zmieniony", 4 => "Oryginał", 5 => "Poprawiony", _ => "Błąd" };
    private sealed record Adapter(Func<int?, string, bool, int> Save, Action NullInput,
        Func<int[]?, int?> One, Func<int[]?, IReadOnlyList<int>> List, Func<int, string> Label);
}
