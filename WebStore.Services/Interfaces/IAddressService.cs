using System.Linq.Expressions;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Interfaces;

public interface IAddressService
{
    AddressVm AddOrUpdateAddress(AddOrUpdateAddressVm model);
    AddressVm? GetAddress(Expression<Func<Address, bool>> predicate);
    IReadOnlyList<AddressVm> GetAddresses(Expression<Func<Address, bool>>? predicate = null);
    CustomerAddressesVm SetCustomerAddresses(SetCustomerAddressesVm model);
    CustomerAddressesVm GetCustomerAddresses(int customerId);
}
