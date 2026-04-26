using DarkKitchen.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.DataAccess.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber);
        builder.Property(o => o.ClientId).IsRequired();
        builder.Property(o => o.Type).HasConversion<string>().IsRequired();
        builder.Property(o => o.State).HasConversion<string>().IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.LastTransitionDate).IsRequired();
        builder.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");

        builder.Ignore(o => o.Subtotal);
        builder.Ignore(o => o.Tax);
        builder.Ignore(o => o.Total);

        builder.OwnsOne(o => o.DeliveryAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").IsRequired().HasMaxLength(100);
            address.Property(a => a.Number).HasColumnName("DoorNumber").IsRequired().HasMaxLength(10);
            address.Property(a => a.Apartment).HasColumnName("Apartment").HasMaxLength(10);
            address.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(50);
            address.Property(a => a.Country).HasColumnName("Country").IsRequired().HasMaxLength(50);
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
