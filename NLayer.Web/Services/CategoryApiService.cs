using NLayer.Core.DTOs;

namespace NLayer.Web.Services
{
    public class CategoryApiService
    {
        // Bunların hangi urlleri kullanacaklarını start up tarafında belirleyeceğiz.Aynı zamanda bunları bir DI containera da ekleyeceğiz.Çünkü bizim products controller hem category datalarına ihtiyacı var hem de product datalarına ihtiyacı var.Bunları katamanlı mimari kullanmadığı için bizim apileri mvc projemizde  oluşturduğumuz iki class'ı kullanacak.Appsettings tarafında base url'i belirleriz.
        // HttpClient'ı kendimiz üretmemeliyiz.Best practices bir yöntem değildir.Her zaman bu işlemi DI container'a bırkmalıyız.Biz kendimiz üretmeyiz DI container bizim program.cs de eklemiş olduklarımızı alır ve ilgili class'ın constructoruna nesne örneği ekledikten sonra daha performanslı ve daha az nesne örneği üreterek httpclient'ı kullanabiliriz.Bu sayede de soket yokluğu gibi hatalardan da arınmış oluruz.
        private readonly HttpClient _httpClient;

        public CategoryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Categori de tek bir endpointe istek yapacağız.O da tüm kategorileri alacağımız endpoint olacak.

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CustomResponseDto<List<CategoryDto>>>("categories");
            return response.Data;
        }
    }
}
