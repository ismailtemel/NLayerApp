using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core;

namespace NLayer.Repository.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        //AppDbContext de yaptığımız işlemi aynen burada yapabiliriz
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            // Bir bir artmasını istiyorsak sütunun aşağıdaki kod satırını uygularız hatta kaçar kaçar artmasını istiyorsak onu da ayarlarız
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

            // Aşağıdaki kod satırında tablomuzun ismini değiştirebilriz.
            // Eğer değiştirmek istemessek default olarak dbsetteki ismini alır.
            builder.ToTable("Categories");

            
        }
    }
}
