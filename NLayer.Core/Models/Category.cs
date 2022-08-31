namespace NLayer.Core.Models
{
    public class Category : BaseEntity
    {
        // Category de birden fazla product olabilir bunun için ICollection kullanırız aşağıdaki gibi.
        // Entityler içerisindeki farklı classlara ve farklı entitylere referans verdiğimiz propertylere biz navigation property deriz neden navigation çünkü category den productlara gidebiliriz yani categorye bağlı tüm productları çekebiliriz
        // Best practiceslerden biri entitylerimizi mümkün olduğunca temiz tutmaktır.
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
