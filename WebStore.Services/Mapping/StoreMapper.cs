using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Mapping;

// Jawne mapowanie chroni identyfikatory i relacje przed przypadkowym nadpisaniem.
public class StoreMapper
{
    public ProductVm Map(Product p) => new(p.Id, p.Name, p.Description, p.ImageBytes.ToArray(),
        p.Price, p.Weight, p.CategoryId, p.SupplierId, p.Stocks.Sum(s => (long)s.Quantity));
    public void Apply(AddOrUpdateProductVm v, Product p)
    {
        p.Name = v.Name; p.Description = v.Description; p.ImageBytes = v.ImageBytes.ToArray();
        p.Price = v.Price; p.Weight = v.Weight; p.CategoryId = v.CategoryId; p.SupplierId = v.SupplierId;
    }
    public AddressVm Map(Address a) => new(a.Id, a.Country, a.PostalCode, a.City,
        a.Street, a.BuildingNumber, a.ApartmentNumber, a.CustomerId, a.StationaryStoreId);
    public void Apply(AddOrUpdateAddressVm v, Address a)
    {
        a.Country = v.Country; a.PostalCode = v.PostalCode; a.City = v.City;
        a.Street = v.Street; a.BuildingNumber = v.BuildingNumber; a.ApartmentNumber = v.ApartmentNumber;
        a.CustomerId = v.CustomerId; a.StationaryStoreId = v.StationaryStoreId;
    }
    public StoreVm Map(StationaryStore s) => new(s.Id, s.Name,
        s.Addresses.Select(a => a.Id).ToArray(), s.Employees.Select(e => e.Id).ToArray());
    public void Apply(AddOrUpdateStoreVm v, StationaryStore s) => s.Name = v.Name;
    public OrderVm Map(Order o) => new(o.Id, o.CustomerId, o.OrderDate, o.DeliveryDate,
        o.TrackingNumber, o.TotalAmount, o.InvoiceId,
        o.OrderProducts.Select(p => new OrderLineVm(p.ProductId, p.Quantity)).ToArray());
    public void Apply(AddOrUpdateOrderVm v, Order o)
    {
        o.CustomerId = v.CustomerId; o.OrderDate = v.OrderDate;
        o.DeliveryDate = v.DeliveryDate; o.TrackingNumber = v.TrackingNumber;
    }
    public InvoiceVm Map(Invoice i) => new(i.Id, i.Number, i.IssueDate, i.Amount, i.Orders.Select(o => o.Id).ToArray());
    public void Apply(AddOrUpdateInvoiceVm v, Invoice i) { i.Number = v.Number; i.IssueDate = v.IssueDate; }
}
