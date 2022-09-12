using NLayer.Core.DTOs;

namespace NLayer.Web.Services
{
    // genel olarak aşağıda istek yaptığımız yerlerdeki parantezler içerisine full url yazmamamızın sebebi full url'i program.cs dosyasında yazmış olmamamız.
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //İlk olarak product ile beraber kategorileri alıyoruz.Bizim ana productcontroller'ımız GetProductsWithCategory'i kullanıyor.Buna burda da ihtiyacımız var bu datayı apiden çekeceğiz.

        public async Task<List<ProductWithCategoryDto>> GetProductsWithCategoryAsync()
        {
            // Önceden aşağıdaki gibi yapıyorduk.
            // Get isteği ile önce datayı alıyorduk.
            // Biz bu response üzerinden context'i okuyorduk.
            // Response'ı issuccess mi diye kontrol ediyorduk.Eğer başarılıysa bunun üzerinden context'ini readasstringle beraber okuyorduk okuduktan sonra stirng'i de jsona cast ediyorduk.Artık aşağıdakilere gerek yok.
            //var response2 = await _httpClient.GetAsync("products/GetProductsWithCategory");


            // httpclinet nesnesi ile getfromasync ile direk datayı async olarak alıyor.Api de bize GetProductsWithCatgeory'den CustomResponseDto<List<ProductWithCategoryDto>>> geliyor.Aşağıdaki bizim beklemiş olduğumuz tiptir.Parantez içine nereye istek yapmamız gerektiğini yazarız.
            var response = await _httpClient.GetFromJsonAsync<CustomResponseDto<List<ProductWithCategoryDto>>>("products/GetProductsWithCategory");
            // Aşağıda response.Data dedikten sonra istemiş oldupumuz datayı döneriz.
            return response.Data;
        }

        public async Task<ProductDto> SaveAsync(ProductDto newProduct)
        {
            // Aşağıda direk json olarak data göndeririz.Parantez products endpointine gideceğini yazarız.Slash diyip birşey vermemize gerek yok buraya direk post isteği yaptığımız da apide bulunan productscontroller içindeki Save'in post endpointi çalışıyor.Methodun ismi önemli değil.
            // Aşağıda products ile birlikte bir data da göndeririz.Bu data newProduct'dır.
            var response = await _httpClient.PostAsJsonAsync("products", newProduct);

            // Arkasından gelen response IsSuccessStatusCode'u false ise geriye null dön demkki kayıt yapılmamış.Arkasından eğer yoluna devam ediyorsa yani başarılı ise bu sefer body'sini okumamız gerekiyor.
            if (!response.IsSuccessStatusCode) return null;

            // Aşağıdaki responseBody den bize string dönüyor..net 5.0 ile gelen aşağıdaki ReadfromJsonAsync ile beraber direk bize json olarak okur.ReadFromJsonAsync'yi seçtikten sonra bize hangi datayı dönemsini istediğimizi de belirtmemiz gerekiyor.Arkasından return ile beraber responseBody'nin datasını dönebiliriz.
            var responseBody = await response.Content.ReadFromJsonAsync<CustomResponseDto<ProductDto>>();

            //Eğer IsSuccessStatusCode'u başarıyla geçtiyse datayı dönebilriz aşağıdaki gibi.Data 200 dönüyorsa mutlaka datası doludur.Aynı şekilde bu method içerisinde loglamasını da yapabiliriz.Bize kalmıştır.Direk hatayı da dönebiliriz.
            return responseBody.Data;
        }

        // Aşağıda datasını dönememize gerek yok true veya false olarak bool değer dönebiliriz.
        public async Task<bool> UpdateAsync(ProductDto newProduct)
        {
            // Aşağıda direk json olarak data göndeririz.Parantez products endpointine gideceğini yazarız.Slash diyip birşey vermemize gerek yok buraya direk post isteği yaptığımız da apide bulunan productscontroller içindeki Save'in post endpointi çalışıyor.Methodun ismi önemli değil.
            // Aşağıda products ile birlikte bir data da göndeririz.Bu data newProduct'dır.
            var response = await _httpClient.PutAsJsonAsync("products", newProduct);

            // Arkasından gelen response IsSuccessStatusCode'u false ise geriye null dön demkki kayıt yapılmamış.Arkasından eğer yoluna devam ediyorsa yani başarılı ise bu sefer body'sini okumamız gerekiyor.
            // Updatede 204 nocontent geldiği için aşağıdaki kodlara gerek kalmaz.
            //if (!response.IsSuccessStatusCode) return null;

            // Aşağıdaki responseBody den bize string dönüyor..net 5.0 ile gelen aşağıdaki ReadfromJsonAsync ile beraber direk bize json olarak okur.ReadFromJsonAsync'yi seçtikten sonra bize hangi datayı dönemsini istediğimizi de belirtmemiz gerekiyor.Arkasından return ile beraber responseBody'nin datasını dönebiliriz.
            //var responseBody = await response.Content.ReadFromJsonAsync<CustomResponseDto<ProductDto>>();

            // Eğer aşağıdaki responseden 200 durum kodu geliyorsa true gelmiyorsa false döneriz.
            // Aynsı zamanda response'ın içeriğini okuyarak da gelen hatayı da bir yere loglayabiliriz.
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            // Delete de direk olarak deleteasync'yi çağırırız parametre göndermeyeceğiz.Burada tek gideceği şey sileceğimiz id'dir.
            var response = await _httpClient.DeleteAsync($"products/{id}");

            // Yine yukarıdaki responsedan başarılı geliyorsa IsSuccessStatusCode true gelmiyorsa false gelir.
            return response.IsSuccessStatusCode;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            // Aşağıda parantez içinde nereye istek yapmak istiyorsak orayı yazarız değişken gireceğimiz parantez içlerine dolar işareti koyarız.

            var response = await _httpClient.GetFromJsonAsync<CustomResponseDto<ProductDto>>($"products/{id}");
            return response.Data;
        }

    }
}
