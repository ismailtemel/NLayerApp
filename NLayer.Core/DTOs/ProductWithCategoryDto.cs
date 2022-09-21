namespace NLayer.Core.DTOs
{
    public class ProductWithCategoryDto : ProductDto
    {
        // Artık IProductService için döneceğimiz dtomuz da hazır.
        // Dikkat edersek repositoryler geriye entity dönerken servisler direk olarak api'nin isteyeceği dto'yu otomatik bir şekilde döner.Yani api'ler tam istediğimiz dataları döner.Yani bir daha api tarafında birşeyleri değiştirmekle uğraşmıyoruz.
        public CategoryDto Category { get; set; }
    }
}
