using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRespository
    {
        // Aşağıda context'le ilgili bir constructor oluşturduk. Eğer oluşturmassak ProductRepository'nin altı kırmızı olur ve hata alırız.
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductListWithCategory()
        {
            // GenericRepository de context'i protected olarak belirtmiştik şimdi o context'i burda kullanacağız.
            // Biz burda include methoduyla birlikte aslında burda eager loading yaptık yani daha datayı çekerken kategorilerin de alınmasını istedik.
            // Efcore da lazy loading de vardır.Eğer product'a bağlı kategoriyi de ihtiyaç olduğunda daha sonra çekersek bu da lazy loading olur.
            // İlk product'ları çektiğimiz anda kategoriyi de çekersek o eager loading olur.
            // 
            return await _context.Products.Include(x=>x.Category).ToListAsync();
        }
    }
}
