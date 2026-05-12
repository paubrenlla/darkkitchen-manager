using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic;

public class ShippingCostCalculator(IShippingTypeRepository shippingTypeRepository) : IShippingCostCalculator
{
    private readonly IShippingTypeRepository _shippingTypeRepository = shippingTypeRepository;

    public decimal CalculateShippingCost(string shippingTypeName)
    {
        var shippingType = _shippingTypeRepository.GetByName(shippingTypeName)
                           ?? throw new ArgumentException($"Tipo de envío '{shippingTypeName}' no existe.");

        return shippingType.Cost;
    }
}
