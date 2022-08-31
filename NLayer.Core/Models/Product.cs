namespace NLayer.Core.Models
{
    public class Product : BaseEntity
    {
        //public Product(string name)
        //{
        //    Name = name ?? throw new ArgumentNullException(nameof(Name));
        //}

        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        // Product'ın 1 tane kategorisi olduğu için aşağıdaki gibi bir isimlendirme yaptık.
        // Aşağıda yazdığımız propertyler birer navigationdur
        public Category Category { get; set; }
        public ProductFeature ProductFeature { get; set; }
    }
}
