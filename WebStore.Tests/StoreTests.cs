using Microsoft.EntityFrameworkCore;
using WebStore.Model.DataModels;
using WebStore.Tests.Infrastructure;

namespace WebStore.Tests;

[Collection("SQL")]
public class StoreTests(SqlDatabase database) : ServiceTest(database)
{
    [Fact]
    public void Store_lists_addresses_and_employees_and_reassigns_employee()
    {
        var secondEmployee = new StationaryStoreEmployee { StationaryStoreId = Store.Id, FirstName = "Adam", LastName = "Testowy", RegistrationDate = Day };
        Db.Add(secondEmployee); Db.SaveChanges(); Db.ChangeTracker.Clear();
        var first = AddressInput(); first.CustomerId = null; first.StationaryStoreId = Store.Id;
        var a = Addresses.AddOrUpdateAddress(first);
        var b = Addresses.AddOrUpdateAddress(first);
        var saved = Stores.GetStore(s => s.Id == Store.Id)!;
        Assert.Equal(new[] { a.Id, b.Id }.Order(), saved.AddressIds.Order());
        Assert.Equal(new[] { Employee.Id, secondEmployee.Id }.Order(), saved.EmployeeIds.Order());
        var other = Stores.AddOrUpdateStore(new() { Name = "Poznań" });
        Assert.Contains(Employee.Id, Stores.AssignEmployee(new(Employee.Id, other.Id)).EmployeeIds);
        Assert.DoesNotContain(Employee.Id, Stores.GetStore(s => s.Id == Store.Id)!.EmployeeIds);
        Assert.Contains(secondEmployee.Id, Stores.GetStore(s => s.Id == Store.Id)!.EmployeeIds);
        Assert.Single(Stores.AssignEmployee(new(Employee.Id, other.Id)).EmployeeIds);
        using var db = Database.Open();
        Assert.Equal(other.Id, db.Employees.Single(e => e.Id == Employee.Id).StationaryStoreId);
        Assert.Equal(2, db.Employees.Count());
    }

    [Fact]
    public void Assignment_rejects_null_missing_and_wrong_type_without_moving_employee()
    {
        Rejected<ArgumentNullException>(() => Stores.AssignEmployee(null!));
        Rejected<KeyNotFoundException>(() => Stores.AssignEmployee(new(Employee.Id, int.MaxValue)));
        Rejected<KeyNotFoundException>(() => Stores.AssignEmployee(new(int.MaxValue, Store.Id)));
        Rejected<ArgumentException>(() => Stores.AssignEmployee(new(Customer.Id, Store.Id)));
        Assert.Contains(Employee.Id, Stores.GetStore(s => s.Id == Store.Id)!.EmployeeIds);
    }
}
