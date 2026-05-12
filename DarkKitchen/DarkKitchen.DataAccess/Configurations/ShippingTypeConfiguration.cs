using DarkKitchen.Domain.Orders.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.DataAccess.Configurations;

public class ShippingTypeConfiguration : IEntityTypeConfiguration<ShippingType>
{
    public void Configure(EntityTypeBuilder<ShippingType> builder)
    {
        builder.ToTable("ShippingTypes");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.Name).IsUnique();
        builder.Property(s => s.Cost).HasColumnType("decimal(18,2)").IsRequired();
    }
}
