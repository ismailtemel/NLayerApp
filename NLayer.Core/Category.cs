using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core
{
    public class Category : BaseEntity
    {
        // Category de birden fazla product olabilir bunun için ICollection kullanırız aşağıdaki gibi.
        // Entityler içerisindeki farklı classlara ve farklı entitylere referans verdiğimiz propertylere b  iz navigation property deriz neden navigation çünkü category den productlara gidebiliriz yani categorye bağlı tüm productları çekebiliriz
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
