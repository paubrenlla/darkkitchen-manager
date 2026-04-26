using DarkKitchen.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.DataAccess.Configurations;

public class ProductLineConfiguration : IEntityTypeConfiguration<ProductLine>
{
    public void Configure(EntityTypeBuilder<ProductLine> builder)
    {
        builder.ToTable("ProductLines");
        builder.HasKey(pl => pl.Id);
        builder.Property(pl => pl.Name).IsRequired().HasMaxLength(50);
    }
}
