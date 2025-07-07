using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.Branch).IsRequired().HasMaxLength(100);
        builder.Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.IsCancelled).IsRequired();
        builder.HasMany(s => s.Items).WithOne().OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(s => s.Customer, cb =>
        {
            cb.Property(c => c.Id).HasColumnName("Customer_Id");
            cb.Property(c => c.Name).HasColumnName("Customer_Name");
            cb.Property(c => c.Email).HasColumnName("Customer_Email");
        });
    }
}