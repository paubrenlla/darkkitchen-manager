using DarkKitchen.Domain.Products;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(Guid id);
    void Add(Product product);
    void Update(Guid id, Product product);
    IEnumerable<ProductLine> GetAllLines();
    IEnumerable<ProductCategory> GetAllCategories();
}
