using DarkKitchen.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.DataAccess.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.ProductId).IsRequired();
        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(oi => oi.DiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Property(oi => oi.AppliedPromotionName).HasMaxLength(50);
    }
}
