using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core.Models;

namespace NLayer.Repository.Seed
{
    internal class ProductSeed : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(new Product
            {
                Id = 1,
                CategoryId= 1,
                Name="Kalem1",
                Price=100,
                Stock=20,
                //Her product eklediğimizde createdDate'i vermek zorunda değiliz merkezi bir yerden çağırabiliriz.Bunu çağırmak için DbContext'e bir interceptor yazarız.Product veya herhangi bir entity kaydedilmeden önce CreatedDate'i set'leriz.
                CreatedDate=DateTime.Now
                // BaseEntity'deki update veri tabanına ilk kayıt eklendiği esnada null olmalı.
                // Update Date 'i de create date gibi merkezi bir yerden her hangi bir entity güncellenmeden önce updateed date'ini set edeceğiz bunu da bir dbcontext nesnemize yani bizim AppDbContext bir interceptor yazacağız araya girici bir method yazacağız aslında yapmak istediğimiz savechange methodunu ovveride etmek.
            },
            new Product
            {
                Id = 2,
                CategoryId = 1,
                Name = "Kalem2",
                Price = 200,
                Stock = 30,
                CreatedDate = DateTime.Now
            }, 
            new Product
            {
                Id = 3,
                CategoryId = 1,
                Name = "Kalem3",
                Price = 300,
                Stock = 40,
                CreatedDate = DateTime.Now
            },
            new Product
            {
                Id = 4,
                CategoryId = 2,
                Name = "Kitaplar1",
                Price = 6300,
                Stock = 140,
                CreatedDate = DateTime.Now
            },
            new Product
            {
                Id = 5,
                CategoryId = 2,
                Name = "Kitaplar2",
                Price = 1300,
                Stock = 240,
                CreatedDate = DateTime.Now
            }, 
            new Product
            {
                Id = 6,
                CategoryId = 2,
                Name = "Kitaplar3",
                Price = 2300,
                Stock = 340,
                CreatedDate = DateTime.Now
            });
        }
    }
}
