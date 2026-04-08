using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
}
