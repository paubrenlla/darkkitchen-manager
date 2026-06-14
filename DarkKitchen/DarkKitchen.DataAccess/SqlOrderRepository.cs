using DarkKitchen.Domain.Orders;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class SqlOrderRepository(DarkKitchenContext context) : IOrderRepository
{
    private readonly DarkKitchenContext _context = context;

    public void Add(Order order)
    {
        var nextNumber = _context.Orders.Count() + 1;
        order.AssignOrderNumber(nextNumber);
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public void Update(Order order)
    {
        var existing = _context.Orders
            .FirstOrDefault(o => o.Id == order.Id)
            ?? throw new KeyNotFoundException($"Pedido {order.Id} no encontrado.");

        existing.TransitionTo(order.State);
        _context.SaveChanges();
    }

    public Order? GetById(Guid id)
    {
        return _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetByClient(Guid clientId, DateTime? from = null, DateTime? to = null, string? state = null)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.ClientId == clientId);

        if(from.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= from.Value);
        }

        if(to.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= to.Value);
        }

        if(!string.IsNullOrWhiteSpace(state) && Enum.TryParse<OrderState>(state, out var parsedState))
        {
            query = query.Where(o => o.State == parsedState);
        }

        return query.ToList();
    }

    public IEnumerable<Order> GetByStatus(DateTime from, DateTime to, string? state = null, string? address = null)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to);

        if(!string.IsNullOrWhiteSpace(state) && Enum.TryParse<OrderState>(state, out var parsedState))
        {
            query = query.Where(o => o.State == parsedState);
        }

        if(!string.IsNullOrWhiteSpace(address))
        {
            query = query.Where(o => o.DeliveryAddress.Street.Contains(address));
        }

        return query.ToList();
    }

    public IEnumerable<Order> GetAll()
    {
        return _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ToList();
    }
}
