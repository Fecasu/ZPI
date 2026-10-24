using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Services.Mapping;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class StoreService(ApplicationDbContext context, StoreMapper mapper, ILogger<StoreService> logger)
    : BaseService(context, mapper, logger), IStoreService
{
    private IQueryable<StationaryStore> Query => Context.Stores.Include(s => s.Addresses).Include(s => s.Employees).AsSplitQuery();

    public StoreVm AddOrUpdateStore(AddOrUpdateStoreVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidId(model.Id);
        var store = model.Id is > 0
            ? Query.SingleOrDefault(s => s.Id == model.Id) ?? throw new KeyNotFoundException("Nie znaleziono sklepu.")
            : new StationaryStore();
        RequiredText(model.Name, nameof(model.Name));
        Mapper.Apply(model, store);
        if (model.Id is null or 0) Context.Stores.Add(store);
        Context.SaveChanges();
        return Mapper.Map(store);
    });

    public StoreVm? GetStore(Expression<Func<StationaryStore, bool>> predicate) => One(Query, predicate, Mapper.Map);
    public IReadOnlyList<StoreVm> GetStores(Expression<Func<StationaryStore, bool>>? predicate = null) => Many(Query, predicate, Mapper.Map);

    public StoreVm AssignEmployee(AssignEmployeeVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        var store = Query.SingleOrDefault(s => s.Id == model.StoreId) ?? throw new KeyNotFoundException("Nie znaleziono sklepu.");
        var employee = RequiredUser<StationaryStoreEmployee>(model.EmployeeId);
        employee.StationaryStore = store;
        employee.StationaryStoreId = store.Id;
        Context.SaveChanges();
        return Mapper.Map(store);
    });
}
