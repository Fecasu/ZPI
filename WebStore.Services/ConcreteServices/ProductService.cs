using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Services.Mapping;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class ProductService(ApplicationDbContext context, StoreMapper mapper, ILogger<ProductService> logger)
    : BaseService(context, mapper, logger), IProductService
{
    private IQueryable<Product> Query => Context.Products.Include(x => x.Stocks);

    public ProductVm AddOrUpdateProduct(AddOrUpdateProductVm model) => Write(() =>
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidId(model.Id);
        var product = model.Id is > 0
            ? Query.SingleOrDefault(p => p.Id == model.Id) ?? throw new KeyNotFoundException("Nie znaleziono produktu.")
            : new Product();
        RequiredText(model.Name, nameof(model.Name));
        RequiredText(model.Description, nameof(model.Description));
        if (model.ImageBytes is null || model.ImageBytes.Length == 0 || model.Price <= 0 || model.Weight <= 0 || !float.IsFinite(model.Weight))
            throw new ArgumentException("Wymagany obraz oraz dodatnia cena i masa.");
        Pln.Validate(model.Price, nameof(model.Price));
        if (!Context.Categories.Any(c => c.Id == model.CategoryId)) throw new KeyNotFoundException("Nie znaleziono kategorii.");
        RequiredUser<Supplier>(model.SupplierId);
        Mapper.Apply(model, product);
        if (model.Id is null or 0) Context.Products.Add(product);
        Context.SaveChanges();
        return Mapper.Map(product);
    });

    public ProductVm? GetProduct(Expression<Func<Product, bool>> predicate) => One(Query, predicate, Mapper.Map);
    public IReadOnlyList<ProductVm> GetProducts(Expression<Func<Product, bool>>? predicate = null) => Many(Query, predicate, Mapper.Map);
}
