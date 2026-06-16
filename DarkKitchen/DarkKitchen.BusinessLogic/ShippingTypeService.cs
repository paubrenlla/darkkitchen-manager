using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ShippingTypeService(IShippingTypeRepository shippingTypeRepository) : IShippingTypeService
{
    private readonly IShippingTypeRepository _shippingTypeRepository = shippingTypeRepository;

    public IEnumerable<ShippingType> GetAll()
    {
        return _shippingTypeRepository.GetAll();
    }

    public ShippingType Create(ShippingTypeRequest request)
    {
        var shippingType = new ShippingType(request.Name, request.Cost);
        _shippingTypeRepository.Add(shippingType);
        return shippingType;
    }

    public ShippingType Update(Guid id, ShippingTypeRequest request)
    {
        var existing = _shippingTypeRepository.GetById(id)
                       ?? throw new KeyNotFoundException($"Tipo de envío {id} no encontrado.");

        existing.UpdateDetails(request.Name, request.Cost);
        _shippingTypeRepository.Update(existing);
        return existing;
    }

    public void Delete(Guid id)
    {
        _shippingTypeRepository.Delete(id);
    }
}
