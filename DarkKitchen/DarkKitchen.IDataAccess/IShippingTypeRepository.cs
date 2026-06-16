using DarkKitchen.Domain.Orders.Delivery;

namespace DarkKitchen.IDataAccess;

public interface IShippingTypeRepository
{
    IEnumerable<ShippingType> GetAll();
    ShippingType? GetById(Guid id);
    ShippingType? GetByName(string name);
    void Add(ShippingType shippingType);
    void Update(ShippingType shippingType);
    void Delete(Guid id);
}
