using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core
{
    public class ProductFeature
    {
        // ProductFeature product'a bağlı olduğu için base entity den referans almamıza gerek kalmaz.
        public int Id { get; set; }
        public string Color { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        //ProductFeature da bir producta ait olacağı için ProductId de tutmamız gerekir.
        public int ProductId { get; set; }
        public Product Product { get; set; }    
    }
}
