using System.Linq.Expressions;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Interfaces;

public interface IStoreService
{
    StoreVm AddOrUpdateStore(AddOrUpdateStoreVm model);
    StoreVm? GetStore(Expression<Func<StationaryStore, bool>> predicate);
    IReadOnlyList<StoreVm> GetStores(Expression<Func<StationaryStore, bool>>? predicate = null);
    StoreVm AssignEmployee(AssignEmployeeVm model);
}
