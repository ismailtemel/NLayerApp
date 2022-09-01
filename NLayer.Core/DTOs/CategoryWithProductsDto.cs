namespace NLayer.Core.DTOs
{
    public class CategoryWithProductsDto : CategoryDto
    {
        //Bizim categorydto'muz vardı fakat categorydtomuz bize product dönmüyordu bunun için bu dto'yu oluşturduk.
        public List<ProductDto> Products { get; set; }
    }
}
