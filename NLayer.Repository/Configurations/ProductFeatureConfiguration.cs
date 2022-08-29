using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core;

namespace NLayer.Repository.Configurations
{
    internal class ProductFeatureConfiguration : IEntityTypeConfiguration<ProductFeature>
    {
        public void Configure(EntityTypeBuilder<ProductFeature> builder)
        {
            builder.HasKey(x => x.Id);
            // Her methoddan sonra "." dediğimizde yeni şeyler tanımlayabiliriz.
            builder.Property(x => x.Id).UseIdentityColumn();
            // Bire bir ilişki olduğu için kimde foreign key olduğunu belirtmemiz gerekir.
            builder.HasOne(x => x.Product).WithOne(x => x.ProductFeature).HasForeignKey<ProductFeature>(x => x.ProductId);
        }
    }
}
