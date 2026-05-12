using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IShippingTypeService
{
    IEnumerable<ShippingTypeResponse> GetAll();
    ShippingTypeResponse Create(ShippingTypeRequest request);
    ShippingTypeResponse Update(Guid id, ShippingTypeRequest request);
    void Delete(Guid id);
}
