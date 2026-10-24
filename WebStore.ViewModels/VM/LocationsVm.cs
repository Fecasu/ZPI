namespace WebStore.ViewModels.VM;

public class AddOrUpdateAddressVm
{
    public int? Id { get; set; }
    public string Country { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string City { get; set; } = "";
    public string? Street { get; set; }
    public string BuildingNumber { get; set; } = "";
    public string? ApartmentNumber { get; set; }
    public int? CustomerId { get; set; }
    public int? StationaryStoreId { get; set; }
}

public record AddressVm(int Id, string Country, string PostalCode, string City,
    string? Street, string BuildingNumber, string? ApartmentNumber, int? CustomerId, int? StationaryStoreId);
public record CustomerAddressesVm(int CustomerId, AddressVm? BillingAddress, AddressVm? ShippingAddress);
public record SetCustomerAddressesVm(int CustomerId, int? BillingAddressId, int? ShippingAddressId);

public class AddOrUpdateStoreVm
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
}

public record StoreVm(int Id, string Name, IReadOnlyList<int> AddressIds, IReadOnlyList<int> EmployeeIds);
public record AssignEmployeeVm(int EmployeeId, int StoreId);
