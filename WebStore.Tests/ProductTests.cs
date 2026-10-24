using Microsoft.EntityFrameworkCore;
using WebStore.Model.DataModels;
using WebStore.Tests.Infrastructure;

namespace WebStore.Tests;

[Collection("SQL")]
public class ProductTests(SqlDatabase database) : ServiceTest(database)
{
    [Fact]
    public void Product_roundtrip_preserves_all_fields_and_relations()
    {
        var input = ProductInput();
        var created = Products.AddOrUpdateProduct(input);
        Assert.Equal(input.Name, created.Name);
        Assert.Equal(input.Description, created.Description);
        Assert.Equal(input.ImageBytes, created.ImageBytes);
        Assert.Equal(input.Price, created.Price);
        Assert.Equal(input.Weight, created.Weight);
        using var db = Database.Open();
        var saved = db.Products.Include(p => p.Category).Include(p => p.Supplier).Single();
        Assert.Equal(Category.Id, saved.Category.Id);
        Assert.Equal("kawa", saved.Category.Tag);
        Assert.Equal(Supplier.Id, saved.Supplier.Id);
        input.Id = saved.Id;
        input.Price = 12.50m;
        Assert.Equal(12.50m, Products.AddOrUpdateProduct(input).Price);
        Assert.Equal(12.50m, Products.GetProduct(p => p.Id == saved.Id)!.Price);
        Assert.Single(Products.GetProducts());
    }

    [Fact]
    public void Get_all_returns_exactly_two_and_stock_is_sum_on_single_and_list_reads()
    {
        var one = Product();
        var two = Product(15, "Herbata");
        Db.ProductStocks.AddRange(new ProductStock { ProductId = one.Id, Quantity = 2 }, new ProductStock { ProductId = one.Id, Quantity = 3 });
        Db.SaveChanges();
        Db.ChangeTracker.Clear();
        Assert.Equal(2, Products.GetProducts().Count);
        Assert.Equal(5, Products.GetProduct(p => p.Id == one.Id)!.Quantity);
        Assert.Equal(0, Products.GetProduct(p => p.Id == two.Id)!.Quantity);
        Assert.Equal(new[] { one.Id, two.Id }.Order(), Products.GetProducts().Select(p => p.Id).Order());
        Assert.Equal(5, Products.GetProducts(p => p.Price == 10).Single().Quantity);
        Assert.Equal(two.Id, Products.GetProducts(p => p.Name == "Herbata").Single().Id);
    }

    [Theory]
    [InlineData("Name")][InlineData("Description")][InlineData("ImageNull")][InlineData("ImageEmpty")]
    [InlineData("PriceZero")][InlineData("PriceNegative")][InlineData("WeightZero")][InlineData("WeightNegative")]
    [InlineData("WeightNaN")][InlineData("WeightInfinity")]
    public void Invalid_product_data_cannot_change_existing_product(string field)
    {
        var product = Product();
        var v = ProductInput(product.Id, "Zmieniony");
        switch (field)
        {
            case "Name": v.Name = " "; break;
            case "Description": v.Description = "\t"; break;
            case "ImageNull": v.ImageBytes = null!; break;
            case "ImageEmpty": v.ImageBytes = []; break;
            case "PriceZero": v.Price = 0; break;
            case "PriceNegative": v.Price = -1; break;
            case "WeightZero": v.Weight = 0; break;
            case "WeightNegative": v.Weight = -1; break;
            case "WeightNaN": v.Weight = float.NaN; break;
            case "WeightInfinity": v.Weight = float.PositiveInfinity; break;
        }
        Rejected<ArgumentException>(() => Products.AddOrUpdateProduct(v));
    }

    [Theory]
    [InlineData("Category")][InlineData("Supplier")][InlineData("WrongSupplierType")]
    public void Missing_or_wrong_references_do_not_create_product(string reference)
    {
        var v = ProductInput();
        if (reference == "Category") v.CategoryId = int.MaxValue;
        else v.SupplierId = reference == "Supplier" ? int.MaxValue : Customer.Id;
        if (reference == "WrongSupplierType") Rejected<ArgumentException>(() => Products.AddOrUpdateProduct(v));
        else Rejected<KeyNotFoundException>(() => Products.AddOrUpdateProduct(v));
    }
}
