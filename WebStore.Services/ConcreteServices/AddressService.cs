using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Services.Mapping;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class AddressService(ApplicationDbContext context, StoreMapper mapper, ILogger<AddressService> logger)
    : BaseService(context, mapper, logger), IAddressService
{
    public AddressVm AddOrUpdateAddress(AddOrUpdateAddressVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidId(model.Id);
        var address = model.Id is > 0
            ? Context.Addresses.SingleOrDefault(a => a.Id == model.Id) ?? throw new KeyNotFoundException("Nie znaleziono adresu.")
            : new Address();
        RequiredText(model.Country, nameof(model.Country));
        RequiredText(model.PostalCode, nameof(model.PostalCode));
        RequiredText(model.City, nameof(model.City));
        RequiredText(model.BuildingNumber, nameof(model.BuildingNumber));
        if (model.CustomerId.HasValue == model.StationaryStoreId.HasValue)
            throw new ArgumentException("Adres musi mieć dokładnie jednego właściciela.");
        if (model.CustomerId is int customerId) RequiredUser<Customer>(customerId);
        if (model.StationaryStoreId is int storeId && !Context.Stores.Any(s => s.Id == storeId))
            throw new KeyNotFoundException("Nie znaleziono sklepu.");
        if (model.Id is > 0 && (model.CustomerId != address.CustomerId || model.StationaryStoreId != address.StationaryStoreId)
            && Context.Customers.Any(c => c.BillingAddressId == address.Id || c.ShippingAddressId == address.Id))
            throw new ArgumentException("Najpierw usuń wskazanie adresu rozliczeniowego i dostawy.");
        Mapper.Apply(model, address);
        if (model.Id is null or 0) Context.Addresses.Add(address);
        Context.SaveChanges();
        return Mapper.Map(address);
    });

    public AddressVm? GetAddress(Expression<Func<Address, bool>> predicate) => One(Context.Addresses, predicate, Mapper.Map);
    public IReadOnlyList<AddressVm> GetAddresses(Expression<Func<Address, bool>>? predicate = null) => Many(Context.Addresses, predicate, Mapper.Map);

    public CustomerAddressesVm SetCustomerAddresses(SetCustomerAddressesVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        var customer = RequiredUser<Customer>(model.CustomerId);
        var billing = OwnedAddress(model.BillingAddressId, customer.Id);
        var shipping = OwnedAddress(model.ShippingAddressId, customer.Id);
        customer.BillingAddress = billing;
        customer.BillingAddressId = billing?.Id;
        customer.ShippingAddress = shipping;
        customer.ShippingAddressId = shipping?.Id;
        Context.SaveChanges();
        return new CustomerAddressesVm(customer.Id, billing is null ? null : Mapper.Map(billing), shipping is null ? null : Mapper.Map(shipping));
    });

    public CustomerAddressesVm GetCustomerAddresses(int customerId) => Execute(() =>
    {
        RequiredUser<Customer>(customerId);
        var c = Context.Customers.AsNoTracking().Include(c => c.BillingAddress).Include(c => c.ShippingAddress)
            .Single(c => c.Id == customerId);
        return new CustomerAddressesVm(c.Id, c.BillingAddress is null ? null : Mapper.Map(c.BillingAddress),
            c.ShippingAddress is null ? null : Mapper.Map(c.ShippingAddress));
    });

    private Address? OwnedAddress(int? id, int customerId)
    {
        if (id is null) return null;
        var address = Context.Addresses.SingleOrDefault(a => a.Id == id) ?? throw new KeyNotFoundException("Nie znaleziono adresu.");
        if (address.CustomerId != customerId) throw new ArgumentException("Adres nie należy do tego klienta.");
        return address;
    }
}
