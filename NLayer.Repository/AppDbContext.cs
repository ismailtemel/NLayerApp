using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using System.Reflection;

namespace NLayer.Repository
{
    public class AppDbContext : DbContext
    {
        // Constructorun paramtre olarak options almasının nedeni bu options ile beraber veri tabanı yolunu startup dosyasından vereceğiz yani startup dosyasından veritabanı yolunu verebilmek için mutlaka bir dbcontext options alan bir constructor oluşturuyoruz peki ne için options, AppDbContext için options oluştururuz.Arkasından bunu base'deki options'a göndeririz.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Yani demek istediğimiz yeni bir productfeature eklemek istediğimizde mutlaka ya var olan product'a bir feature ekleyebiliriz ya da sıfırdan product ekler gibi aynı zamanda featuresini de ekleyebiliriz.Sonuç olarak dbset olarak eklemek istemeyip product üzerinden feature eklmek istersek aşağıdaki gibi yapabiliriz bu tamamen bize kalmıştır.Öğrenme aşamasında olduğumuz için şuanda dbset üzerinden ilerleyeceğiz fakat best practices açısından product üzerinden productfeature eklemek daha doğrudur.
            //var p = new Product() { ProductFeature = new ProductFeature() { } };
        }
        // Burada her bir entity'mize karşılık olarak bir DbSet oluşturuyoruz.
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        // Burada productFeature product ile ilgili şimdi buraya productfeature'ı eklersek bağımsız olarak productfeature satırlarını db'ye ekleyebiliriz veya güncelleyebiliriz veya yazdığımız aşağıdaki dbset'i yorum satırına alabiliriz eğer bir yazılımcı productfeatureyi eklemek istiyorsa bunu product nesnesi üzerinden eklemeli

        public DbSet<ProductFeature> ProductFeatures { get; set; }

        //Entitylerimizle ilgili ayarları yapabilmek için model oluştururken çalışacak olan methodumuz aşağıdaki methoddur.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Solution exporerdaki her dosya bizim için bir assembly'dir
            // ApplyConfigurationsFromAssembly tüm configuration dosyalarını okur. Nasıl okur peki configurationlara tanımladığımız interface sayesinde okur.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            // Eğer yukardakini kullanmak istemessek aşağıdakini kullanırız fakat bu da bize kod kalabalığı oluşturur.
            // Bunun için yukarıdakini kullanmak daha mantıklıdır.
            // modelBuilder.ApplyConfiguration(new ProductConfiguration());



            //Aşağıdaki kod satırı fluent api olarak adlandırılır yani direk olarak haskey dedikten sonra methodlarımıza devam edebiliriz.Burada property'nin ismini dahi değiştirebiliriz.Best Practices olarak ayarları merkezi bir yerden yapamamız gerekiyor. Gidip de entitylerimize attributelar tanımlamamamız gerekiyor mümkün olduğunca.
            // E şimdi de dbcontext'i kirletmeye başladık burayı da temiz tutmamız gerekiyor bundan dolayı her bir entityle ilgili ayarı farklı class da yapmalıyız.
            // Burada bahsettiğimiz farklı datalar repository katmanındakı seed data klasörüne yazılır.
            // modelBuilder.Entity<Category>().HasKey(x=>x.Id);

            modelBuilder.Entity<ProductFeature>().HasData(new ProductFeature()
            {
                Id = 1,
                Color = "Red",
                Height = 100,
                Width = 200,
                ProductId = 1
            },
            new ProductFeature()
            {
                Id = 2,
                Color = "Yellow",
                Height = 200,
                Width = 300,
                ProductId = 2
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
