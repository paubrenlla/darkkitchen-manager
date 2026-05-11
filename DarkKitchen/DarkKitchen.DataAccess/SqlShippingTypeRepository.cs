using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class SqlShippingTypeRepository(DarkKitchenContext context) : IShippingTypeRepository
{
    private readonly DarkKitchenContext _context = context;

    public IEnumerable<ShippingType> GetAll()
    {
        return _context.ShippingTypes.AsNoTracking().ToList();
    }

    public ShippingType? GetById(Guid id)
    {
        return _context.ShippingTypes.AsNoTracking().FirstOrDefault(s => s.Id == id);
    }

    public ShippingType? GetByName(string name)
    {
        return _context.ShippingTypes.AsNoTracking()
            .FirstOrDefault(s => s.Name == name);
    }

    public void Add(ShippingType shippingType)
    {
        _context.ShippingTypes.Add(shippingType);
        _context.SaveChanges();
    }

    public void Update(ShippingType shippingType)
    {
        var existing = _context.ShippingTypes.FirstOrDefault(s => s.Id == shippingType.Id)
                       ?? throw new KeyNotFoundException($"Tipo de envío {shippingType.Id} no encontrado.");

        existing.UpdateDetails(shippingType.Name, shippingType.Cost);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var existing = _context.ShippingTypes.FirstOrDefault(s => s.Id == id)
                       ?? throw new KeyNotFoundException($"Tipo de envío {id} no encontrado.");

        _context.ShippingTypes.Remove(existing);
        _context.SaveChanges();
    }
}
