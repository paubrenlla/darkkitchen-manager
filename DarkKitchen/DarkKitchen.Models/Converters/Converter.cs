using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.Domain.Users;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.Models.Converters;

public static class Converter
{
    public static LoginResponse ToLoginResponse(string token, User user)
    {
        return new LoginResponse { Token = token, Role = user.Role.ToString() };
    }

    public static ProductResponse ToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Line = product.Line.Name,
            Category = product.Category.Name,
            Images = product.Images.Select(i => i.Url).ToList(),
            IsActive = product.IsActive
        };
    }

    public static UserCreateResponse ToUserCreateResponse(User user)
    {
        return new UserCreateResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Phone = $"{user.Phone.CountryPrefix}{user.Phone.Number}",
            Role = user.Role.ToString()
        };
    }

    public static OrderCreateResponse ToOrderCreateResponse(Order order)
    {
        return new OrderCreateResponse
        {
            Id = order.Id,
            ClientId = order.ClientId,
            OrderNumber = order.OrderNumber ?? 0,
            Subtotal = order.Subtotal,
            ShippingCost = order.ShippingCost,
            Total = order.Total
        };
    }

    public static OrderStatusResponse ToOrderStatusResponse(Order order)
    {
        return new OrderStatusResponse
        {
            Status = order.State.ToString(),
            LastTransitionDate = order.LastTransitionDate
        };
    }

    public static OrderDetailResponse ToOrderDetailResponse(Order order)
    {
        return new OrderDetailResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            CreatedAt = order.CreatedAt,
            Status = order.State.ToString(),
            Total = order.Total,
            Items = order.Items.Select(i => new OrderItemDetailDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                AppliedPromotion = i.AppliedPromotionName,
                ItemTotal = i.CalculateItemTotal(),
            }).ToList(),
        };
    }

    public static OrderListResponse ToOrderListResponse(
        Order order,
        string clientName = "",
        List<OrderItemSummaryDto>? items = null)
    {
        return new OrderListResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientName = clientName,
            CreatedAt = order.CreatedAt,
            Status = order.State.ToString(),
            Total = order.Total,
            ProductCount = order.Items.Sum(i => i.Quantity),
            Items = items ?? [],
        };
    }

    public static PromotionCreateResponse ToPromotionCreateResponse(Promotion promotion)
    {
        return new PromotionCreateResponse
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            Products = promotion.Products.Select(p => p.Code).ToList()
        };
    }

    public static AuditLogResponse ToAuditLogResponse(AuditLog auditLog)
    {
        return new AuditLogResponse
        {
            Id = auditLog.Id,
            Timestamp = auditLog.Timestamp,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            ChangeDescription = auditLog.ChangeDescription,
            ResponsibleUser = auditLog.ResponsibleUser
        };
    }

    public static ShippingTypeResponse ToShippingTypeResponse(ShippingType shippingType)
    {
        return new ShippingTypeResponse
        {
            Id = shippingType.Id,
            Name = shippingType.Name,
            Cost = shippingType.Cost,
        };
    }
}
