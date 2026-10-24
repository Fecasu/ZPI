using Microsoft.AspNetCore.Identity;

namespace WebStore.Model.DataModels;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime RegistrationDate { get; set; }
}

public class Customer : User
{
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public int? BillingAddressId { get; set; }
    public Address? BillingAddress { get; set; }
    public int? ShippingAddressId { get; set; }
    public Address? ShippingAddress { get; set; }
}

public class Supplier : User
{
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class StationaryStoreEmployee : User
{
    public int StationaryStoreId { get; set; }
    public StationaryStore StationaryStore { get; set; } = null!;
}
