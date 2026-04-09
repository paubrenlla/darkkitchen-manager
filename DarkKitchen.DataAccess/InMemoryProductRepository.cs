using DarkKitchen.Domain;
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
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo, categoryParrilla, 150m),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo, categoryParrilla, 200m),
            new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", lineDesayunos, categoryBebidas, 120m),
        ];
    }

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }
}
