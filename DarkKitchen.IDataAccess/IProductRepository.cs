using DarkKitchen.Domain.Products;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(Guid id);
}
