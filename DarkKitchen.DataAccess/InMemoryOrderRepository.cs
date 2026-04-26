using DarkKitchen.Domain.Orders;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];
    private int _nextOrderNumber = 1;
    public IEnumerable<Order> GetAll()
    {
        return _orders;
    }

    public void Add(Order order)
    {
        order.AssignOrderNumber(_nextOrderNumber++);
        _orders.Add(order);
    }

    public void Update(Order order)
    {
        var index = _orders.FindIndex(o => o.Id == order.Id);
        if(index >= 0)
        {
            _orders[index] = order;
        }
    }

    public Order? GetById(Guid id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetByClient(Guid clientId, DateTime? from = null, DateTime? to = null, string? state = null)
    {
        var query = _orders.Where(o => o.ClientId == clientId);
        if(!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(o => o.State.ToString().Equals(state, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }

    public IEnumerable<Order> GetByStatus(DateTime from, DateTime to, string? state = null, string? city = null)
    {
        var query = _orders.Where(o => o.CreatedAt >= from && o.CreatedAt <= to);
        if(!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(o => o.State.ToString().Equals(state, StringComparison.OrdinalIgnoreCase));
        }

        if(!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(o => o.DeliveryAddress.City.Contains(city, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }
}
