using Microsoft.EntityFrameworkCore;
using WebStore.Tests.Infrastructure;
using WebStore.ViewModels.VM;

namespace WebStore.Tests;

[Collection("SQL")]
public class AddressTests(SqlDatabase database) : ServiceTest(database)
{
    [Fact]
    public void Addresses_preserve_fields_and_customer_can_choose_same_different_or_no_addresses()
    {
        var v = AddressInput();
        v.Street = "Leśna"; v.ApartmentNumber = "2";
        var a = Addresses.AddOrUpdateAddress(v);
        var b = Addresses.AddOrUpdateAddress(AddressInput(city: "Gdańsk"));
        Assert.Equal("Leśna", a.Street);
        Assert.Equal("2", a.ApartmentNumber);
        Assert.Null(b.Street);
        Assert.Null(b.ApartmentNumber);
        var selected = Addresses.SetCustomerAddresses(new(Customer.Id, a.Id, b.Id));
        Assert.Equal(a, selected.BillingAddress);
        Assert.Equal(b, selected.ShippingAddress);
        Assert.Equal(selected, Addresses.GetCustomerAddresses(Customer.Id));
        var same = Addresses.SetCustomerAddresses(new(Customer.Id, a.Id, a.Id));
        Assert.Equal(same.BillingAddress, same.ShippingAddress);
        Assert.Equal(same, Addresses.GetCustomerAddresses(Customer.Id));
        var cleared = Addresses.SetCustomerAddresses(new(Customer.Id, null, null));
        Assert.Null(cleared.BillingAddress); Assert.Null(cleared.ShippingAddress);
        Assert.Equal(cleared, Addresses.GetCustomerAddresses(Customer.Id));
        using var db = Database.Open();
        Assert.Equal(2, db.Customers.Include(c => c.Addresses).Single(c => c.Id == Customer.Id).Addresses.Count);
    }

    [Theory]
    [InlineData("Country")][InlineData("PostalCode")][InlineData("City")][InlineData("BuildingNumber")]
    [InlineData("NoOwner")][InlineData("TwoOwners")][InlineData("WrongCustomerType")]
    public void Invalid_address_is_rejected_without_write(string error)
    {
        var a = Addresses.AddOrUpdateAddress(AddressInput());
        var v = AddressInput(a.Id, "Zmiana");
        switch (error)
        {
            case "Country": v.Country = " "; break;
            case "PostalCode": v.PostalCode = ""; break;
            case "City": v.City = null!; break;
            case "BuildingNumber": v.BuildingNumber = "\t"; break;
            case "NoOwner": v.CustomerId = null; break;
            case "TwoOwners": v.StationaryStoreId = Store.Id; break;
            case "WrongCustomerType": v.CustomerId = Supplier.Id; break;
        }
        Rejected<ArgumentException>(() => Addresses.AddOrUpdateAddress(v));
    }

    [Theory]
    [InlineData(true)][InlineData(false)]
    public void Missing_owner_is_rejected(bool customer)
    {
        var v = AddressInput();
        v.CustomerId = customer ? int.MaxValue : null;
        v.StationaryStoreId = customer ? null : int.MaxValue;
        Rejected<KeyNotFoundException>(() => Addresses.AddOrUpdateAddress(v));
    }

    [Theory]
    [InlineData("OtherCustomer")][InlineData("Store")][InlineData("Missing")]
    public void Invalid_second_selection_preserves_both_previous_selections(string owner)
    {
        var a = Addresses.AddOrUpdateAddress(AddressInput());
        var b = Addresses.AddOrUpdateAddress(AddressInput(city: "Łódź"));
        Addresses.SetCustomerAddresses(new(Customer.Id, a.Id, a.Id));
        var foreignInput = AddressInput();
        foreignInput.CustomerId = owner == "OtherCustomer" ? OtherCustomer.Id : null;
        foreignInput.StationaryStoreId = owner == "Store" ? Store.Id : null;
        var invalidId = owner == "Missing" ? int.MaxValue : Addresses.AddOrUpdateAddress(foreignInput).Id;
        if (owner == "Missing") Rejected<KeyNotFoundException>(() => Addresses.SetCustomerAddresses(new(Customer.Id, b.Id, invalidId)));
        else Rejected<ArgumentException>(() => Addresses.SetCustomerAddresses(new(Customer.Id, b.Id, invalidId)));
        Assert.Equal(a.Id, Addresses.GetCustomerAddresses(Customer.Id).BillingAddress!.Id);
        Assert.Equal(a.Id, Addresses.GetCustomerAddresses(Customer.Id).ShippingAddress!.Id);
    }

    [Theory]
    [InlineData(true)][InlineData(false)]
    public void Selected_address_cannot_change_owner_until_selection_is_cleared(bool billing)
    {
        var a = Addresses.AddOrUpdateAddress(AddressInput());
        Addresses.SetCustomerAddresses(new(Customer.Id, billing ? a.Id : null, billing ? null : a.Id));
        var changed = AddressInput(a.Id);
        changed.CustomerId = null; changed.StationaryStoreId = Store.Id;
        Rejected<ArgumentException>(() => Addresses.AddOrUpdateAddress(changed));
        Addresses.SetCustomerAddresses(new(Customer.Id, null, null));
        var moved = Addresses.AddOrUpdateAddress(changed);
        Assert.Null(moved.CustomerId);
        Assert.Equal(Store.Id, moved.StationaryStoreId);
        Assert.Empty(Addresses.GetAddresses(x => x.CustomerId == Customer.Id));
        Assert.Contains(a.Id, Stores.GetStore(s => s.Id == Store.Id)!.AddressIds);
        changed.StationaryStoreId = null; changed.CustomerId = OtherCustomer.Id;
        Assert.Equal(OtherCustomer.Id, Addresses.AddOrUpdateAddress(changed).CustomerId);
    }

    [Fact]
    public void Selection_operations_validate_input_and_customer_type()
    {
        Rejected<ArgumentNullException>(() => Addresses.SetCustomerAddresses(null!));
        Rejected<KeyNotFoundException>(() => Addresses.SetCustomerAddresses(new(int.MaxValue, null, null)));
        Rejected<ArgumentException>(() => Addresses.SetCustomerAddresses(new(Supplier.Id, null, null)));
        Rejected<KeyNotFoundException>(() => Addresses.GetCustomerAddresses(int.MaxValue));
        Rejected<ArgumentException>(() => Addresses.GetCustomerAddresses(Supplier.Id));
    }
}
