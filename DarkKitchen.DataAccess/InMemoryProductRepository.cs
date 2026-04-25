using DarkKitchen.Domain.Products;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    public InMemoryProductRepository()
    {
        var lineCombo = new ProductLine("Combo burgers");
        var lineDesayunos = new ProductLine("Desayunos");
        var categoryParrilla = new ProductCategory("Parrilla");
        var categoryBebidas = new ProductCategory("Bebidas");

        _products =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo, categoryParrilla, 150m, [new ProductImage("default.jpg", 50000)]),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo, categoryParrilla, 200m, [new ProductImage("default.jpg", 50000)]),
            new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", lineDesayunos, categoryBebidas, 120m, [new ProductImage("default.jpg", 50000)]),
        ];
    }

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(Guid id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        _products.Add(product);
    }

    public void Update(Guid id, Product product)
    {
        var index = _products.FindIndex(p => p.Id == id);
        if(index >= 0)
        {
            _products[index] = product;
        }
    }
}
