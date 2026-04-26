using DarkKitchen.Domain;
using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
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
                ItemTotal = i.CalculateItemTotal()
            }).ToList()
        };
    }

    public static OrderListResponse ToOrderListResponse(Order order)
    {
        return new OrderListResponse
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            CreatedAt = order.CreatedAt,
            Status = order.State.ToString(),
            Total = order.Total,
            ProductCount = order.Items.Sum(i => i.Quantity)
        };
    }

    public static PromotionCreateResponse ToPromotionCreateResponse(Promotion promotion)
    {
        return new PromotionCreateResponse
        {
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            Products = promotion.Products.Select(p => p.Code).ToList()
        };
    }

    public static Product ToProduct(ProductCreateRequest request)
    {
        var line = new ProductLine(request.Line);
        var category = new ProductCategory(request.Category);
        var images = request.Images
            .Select(i => new ProductImage(i.Url, i.SizeInBytes))
            .ToList();

        return new Product(request.Code, request.Name, request.Description, line, category, request.Price, images);
    }
}
