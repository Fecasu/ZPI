using System.Linq.Expressions;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Interfaces;

public interface IInvoiceService
{
    InvoiceVm AddOrUpdateInvoice(AddOrUpdateInvoiceVm model);
    InvoiceVm? GetInvoice(Expression<Func<Invoice, bool>> predicate);
    IReadOnlyList<InvoiceVm> GetInvoices(Expression<Func<Invoice, bool>>? predicate = null);
}
