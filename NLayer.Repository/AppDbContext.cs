using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using System.Reflection;

namespace NLayer.Repository
{
    public class AppDbContext : DbContext
    {
        // Constructorun paramtre olarak options almasının nedeni çünkü bu options ile beraber veri tabanı yolunu startup dosyasından vereceğiz yani startup dosyasından veritabanı yolunu verebilmek için mutlaka bir dbcontext options alan bir constructor oluşturuyoruz peki ne için options AppDbContext için options oluştururuz.Arkasından bunu base'deki options'a göndeririz.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Yani demek istediğimiz yeni bir productfeature eklemek istediğimizde mutlaka ya var olan product'a bir feature ekleyebiliriz ya da sıfırdan product ekler gibi aynı zamanda featuresini de ekleyebiliriz.Sonuş olarak dbset olarak eklemek istemeyim product üzerinden feature kelmek istersek aşağıdaki gibi yapabiliriz bu tamamen bize kalmıştır.Öğrenme aşamasında olduğumuz için şuanda dbset üzerinden ilerleyeceğiz fakat best practices açısından product üzerinden productfeature eklemek daha doğrudur.
            //var p = new Product() { ProductFeature = new ProductFeature() { } };
        }
        // Burada her bir entity'mize karşılık olarak bir DbSet oluşturuyoruz.
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        // Burada productFeature product ile ilgili şimdi buraya productfeature'ı eklersek bağımsız olarak productfeature satırlarını db'ye ekleyebiliriz veya güncelleyebiliriz veya yazdığımız aşağıdaki dbset'i yorum satırına alabiliriz eğer bir yazılımcı productfeatureyi eklemek istiyorsa bunu product nesnesi üzerinden eklemeli
        public DbSet<ProductFeature> ProductFeatures { get; set; }

        public override int SaveChanges()
        {
            // İki tane overload olduğu için bazen de savechangeyi çağırabileceğimiz için bu yüzden savechangeyi de almamız lazım.
            foreach (var item in ChangeTracker.Entries())
            {
                // Entity den gelen eğer aşağıdaki is basenetity ise bizim tüm entitylerimiz baseentityden miras alıyordu arkasından bu bir referanstır diyoruz.
                if (item.Entity is BaseEntity entityReference)
                {
                    // Burda bir yanlışlık yaptık item'ın entitylerinde değil item'ın statelerinde döneceğiz.
                    switch (item.State)
                    {
                        // Eğer entity stateler den add ise eğer bir entity eklenme durumu var ise yukarıdaki entitynin referencesinin createddate'ini datetime olarak veriyoruz.
                        case EntityState.Added:
                            {
                                entityReference.CreatedDate = DateTime.Now;
                                break;
                            }
                        case EntityState.Modified:
                            {
                                Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
                                entityReference.UpdatedDate = DateTime.Now;
                                break;
                            }
                    }
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Buralar api için de geçerlidir.
            // Burda veritabanına yansıtmadan önce veritabanına yansıtılacak olan entitylerin createddate ve updateddate'ini güncelledik.
            // Efcore biz savechange'i çağırana kadar tüm entityleri memory'de track ediyordu.Biz savechange'yi çağırdığımız zaman veritabanına yansıtıyordu.Burda veritabanına yansıtmadan hemen önce aşağıda entitynin update'mi edildiğini yoksa yeni insert mü edildiğini anlayacağız ve ona göre de createddate'i ve updateddate'i değiştireceğiz.
            // Foreach ile beraber track etmiş olan entitylerde dönelim.
            foreach (var item in ChangeTracker.Entries())
            {
                // Entity den gelen eğer aşağıdaki is basenetity ise bizim tüm entitylerimiz baseentityden miras alıyordu arkasından bu bir referanstır diyoruz.
                if (item.Entity is BaseEntity entityReference)
                {
                    // Burda bir yanlışlık yaptık item'ın entitylerinde değil item'ın statelerinde döneceğiz.
                    switch (item.State)
                    {
                        // Eğer entity stateler den add ise eğer bir entity eklenme durumu var ise yukarıdaki entitynin referencesinin createddate'ini datetime olarak veriyoruz.
                        case EntityState.Added:
                            {
                                entityReference.CreatedDate = DateTime.Now;
                                break;
                            }
                        case EntityState.Modified:
                            {
                                // Burdaki createddate'i update yaparken db'ye yansıtmamamız lazım yani createddate'i değiştirmemesi lazım o zaman diyoruz ki burda 
                                // Yukarıdaki entity referancenin propertylerine git ve createddate'inin ismodified'ını false yap diyoruz.Bu sayede biz update yaparken createddate'e dokunmadan geçeriz.Burda entity framework'e CreatedDate alanına dokunma diyoruz.CreatedDate alanı neyse db de aynı şekilde kalsın diyoruz.Modified'ını false set ettik.
                                Entry(entityReference).Property(x=>x.CreatedDate).IsModified = false;
                                entityReference.UpdatedDate = DateTime.Now;
                                break;
                            }
                    }
                }
            }



            return base.SaveChangesAsync(cancellationToken);
        }


        //Entitylerimizle ilgili ayarları yapabilmek için model oluşurken çalışacak olan methodumuz aşağıdaki methoddur.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Solution exporerdaki her dosya bizim için bir assembly'dir
            // ApplyConfigurationsFromAssembly bu tüm configuration dosyalarını okur. Nasıl okur peki configurationlara tanımladığımız interface sayesinde okur.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            // Eğer yukardakini kullanmak istemessek aşağıdakini kullanırız fakat bu da bize kod kalabalığı oluşturur.
            // Bunun için yukarıdakini kullanmak daha mantıklıdır.
            //modelBuilder.ApplyConfiguration(new ProductConfiguration());



            //Aşağıdaki kod satırı fluent api olarak adlandırılır yani direk olarak haskey dedikten sonra methodlarımıza devam edebiliriz.Burada property'nin ismini dahi değiştirebiliriz.Best Practices olarak ayarları merkezi bir yerden yapamamız gerekiyor. Gidip de entitylerimize attributelar tanımlamamamız gerekiyor mümkün olduğunca.
            // E şimdi de debcontext'i kirletmeye başladık burayı da temiz tutmamız gerekiyor bundan dolayı her bir entityle ilgili ayarı farklı class da yapmalıyız
            //modelBuilder.Entity<Category>().HasKey(x=>x.Id);

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
