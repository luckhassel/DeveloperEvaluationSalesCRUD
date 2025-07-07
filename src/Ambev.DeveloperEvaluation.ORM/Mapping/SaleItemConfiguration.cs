using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        builder.OwnsOne(i => i.Product, pb =>
        {
            pb.Property(p => p.Id).HasColumnName("Product_Id");
            pb.Property(p => p.Name).HasColumnName("Product_Name");
        });
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.Discount).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(i => i.Total).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.IsCancelled).IsRequired();
    }
}