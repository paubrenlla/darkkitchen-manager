using DarkKitchen.Domain.Orders;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderEnricher
{
    OrderListResponse EnrichForClient(Order order);
    OrderListResponse EnrichForPreparador(Order order);
}
