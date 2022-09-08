using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core.Models;

namespace NLayer.Repository.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            // Her methoddan sonra "." dediğimizde yeni şeyler tanımlayabiliriz.
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Stock).IsRequired();
            // Decimal(18,2) başta 16 karakter virgüldenn sonra 2 karakter olsun demektir. 18 yazılan sayıların toplamına denk geliyor
            builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.ToTable("Products");

            //HasOne ile bir products'ın bir kategorisi olabilir dedik. Withmany ile bir kategorinin de birden fazla products'ı olabilir dedik. HasForeignKey ile de foreign key belirledik.
            builder.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
        }
    }
}
