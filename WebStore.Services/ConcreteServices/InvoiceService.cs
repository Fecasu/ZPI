using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Services.Mapping;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class InvoiceService(ApplicationDbContext context, StoreMapper mapper, ILogger<InvoiceService> logger)
    : BaseService(context, mapper, logger), IInvoiceService
{
    private IQueryable<Invoice> Query => Context.Invoices.Include(i => i.Orders);

    public InvoiceVm AddOrUpdateInvoice(AddOrUpdateInvoiceVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidId(model.Id);
        var invoice = model.Id is > 0
            ? Query.SingleOrDefault(i => i.Id == model.Id) ?? throw new KeyNotFoundException("Nie znaleziono faktury.")
            : new Invoice();
        RequiredText(model.Number, nameof(model.Number));
        if (model.IssueDate == default) throw new ArgumentException("Wymagana data wystawienia.");
        if (Context.Invoices.Any(i => i.Id != invoice.Id && i.Number == model.Number))
            throw new ArgumentException("Numer faktury jest już używany.");
        if (model.OrderIds is null || model.OrderIds.Count == 0) throw new ArgumentException("Faktura wymaga zamówień.");
        var ids = model.OrderIds.Distinct().ToArray();
        var orders = Context.Orders.Where(o => ids.Contains(o.Id)).ToArray();
        if (orders.Length != ids.Length) throw new KeyNotFoundException("Nie znaleziono zamówienia.");
        if (orders.Select(o => o.CustomerId).Distinct().Count() != 1)
            throw new ArgumentException("Zamówienia muszą należeć do jednego klienta.");
        if (orders.Any(o => o.InvoiceId.HasValue && o.InvoiceId != invoice.Id))
            throw new ArgumentException("Zamówienie należy do innej faktury.");
        var amount = orders.Sum(o => o.TotalAmount);
        Mapper.Apply(model, invoice);
        invoice.Amount = amount;
        invoice.Orders.Clear();
        foreach (var order in orders) invoice.Orders.Add(order);
        if (model.Id is null or 0) Context.Invoices.Add(invoice);
        Context.SaveChanges();
        return Mapper.Map(invoice);
    });

    public InvoiceVm? GetInvoice(Expression<Func<Invoice, bool>> predicate) => One(Query, predicate, Mapper.Map);
    public IReadOnlyList<InvoiceVm> GetInvoices(Expression<Func<Invoice, bool>>? predicate = null) => Many(Query, predicate, Mapper.Map);
}
