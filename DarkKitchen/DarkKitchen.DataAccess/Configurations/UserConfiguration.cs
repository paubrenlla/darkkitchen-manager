using DarkKitchen.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Surname).IsRequired().HasMaxLength(25);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.HashedPassword).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().IsRequired();

        builder.OwnsOne(u => u.Phone, phone =>
        {
            phone.Property(p => p.CountryPrefix)
                .HasColumnName("PhoneCountryPrefix")
                .IsRequired()
                .HasMaxLength(5);
            phone.Property(p => p.Number)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(15);
        });
    }
}
