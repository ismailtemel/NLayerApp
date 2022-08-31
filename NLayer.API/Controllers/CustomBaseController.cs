using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;

namespace NLayer.API.Controllers
{
    // Aşağıdaki routelar miras verdiğimiz sınıfta da olduğu için burada gerek kalmaz. Bundan dolayı sileriz.
    //[Route("api/[controller]")]
    //[ApiController]
    public class CustomBaseController : ControllerBase
    {
        // Aşağıdaki kod bir endpoint değildir.Bunu belirtmek için NoAction attribute'ünü ekleriz.Bunu eklemessek swagger bunu bir endpoint gibi algılar ve endpoint olarak algıladığında get veya post'u olmadığı için hata fırlatır.NonAction,bu bir endpoint değil kendi içimde kullanıyorum demektir.
        // Burda bir data dönebilir diye T aldı.Parantez içerisinde ise bizim CustomResponseDto'muzu alır
        [NonAction]
        public IActionResult CreateActionResult<T>(CustomResponseDto<T> response)
        {
            if (response.StatusCode == 204) // 204 durum kodu NoContent demektir.
            {
                // Burda objectresult dönememizin sebebi controller içinde tekrar tekrar ok ,badrequest gibi farklı farklı dönememize gerek kalmaz.Objectresult döneriz döneceğimiz data yı null yaparız.İçerisindeki status kodu ise responseden gelen status kod olsun.Bu sayede her seferinde tek tek dönememize gerek yok.Bu methodu çağırdığımızda response'ın status kodu neyse onu dönecek.
                return new ObjectResult(null)
                {
                    //Burdaki statuscode responsedan gelen status koddur.
                    StatusCode = response.StatusCode,
                };
            }
            // Biz yukarıdaki kodları eğer 204 dönerse yapılacak diye yazdık eğer 204 değilse aşağıdaki kodlar devreye girer.
            // Aşağıdaki kod şu anlama gelir. Eğer statuscode 200 ise geriye 200 dönmüş olacağız eğer burda bize status kod 404 geliyorsa 404 döneceğiz 
            return new ObjectResult(response)
            {
                StatusCode=response.StatusCode
            };
        }
    }
}
