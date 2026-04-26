using DarkKitchen.Domain.Orders;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();
    void Add(Order order);
    void Update(Order order);
    Order? GetById(Guid id);
    IEnumerable<Order> GetByClient(Guid clientId, DateTime? from = null, DateTime? to = null, string? state = null);
    IEnumerable<Order> GetByStatus(DateTime from, DateTime to, string? state = null, string? city = null);
}
