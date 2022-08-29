using NLayer.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            // Burdaki asenkron methodu result property'si ile senkrona çevirebiliriz.Yanlız result property'si threadlik bloklayıcı bir property'dir bu yüzden ayrı bir method olarak tanımlamak daha uygundur.
           await _context.SaveChangesAsync();
        }
    }
}
