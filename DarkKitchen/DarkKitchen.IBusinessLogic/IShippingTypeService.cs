using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IShippingTypeService
{
    IEnumerable<ShippingType> GetAll();
    ShippingType Create(ShippingTypeRequest request);
    ShippingType Update(Guid id, ShippingTypeRequest request);
    void Delete(Guid id);
}
