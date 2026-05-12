using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ShippingTypeService(IShippingTypeRepository shippingTypeRepository) : IShippingTypeService
{
    private readonly IShippingTypeRepository _shippingTypeRepository = shippingTypeRepository;

    public IEnumerable<ShippingTypeResponse> GetAll()
    {
        return _shippingTypeRepository.GetAll().Select(Converter.ToShippingTypeResponse);
    }

    public ShippingTypeResponse Create(ShippingTypeRequest request)
    {
        var shippingType = new ShippingType(request.Name, request.Cost);
        _shippingTypeRepository.Add(shippingType);
        return Converter.ToShippingTypeResponse(shippingType);
    }

    public ShippingTypeResponse Update(Guid id, ShippingTypeRequest request)
    {
        var existing = _shippingTypeRepository.GetById(id)
                       ?? throw new KeyNotFoundException($"Tipo de envío {id} no encontrado.");

        existing.UpdateDetails(request.Name, request.Cost);
        _shippingTypeRepository.Update(existing);
        return Converter.ToShippingTypeResponse(existing);
    }

    public void Delete(Guid id)
    {
        _shippingTypeRepository.Delete(id);
    }
}
