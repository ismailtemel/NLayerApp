using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;

namespace NLayer.API.Filter
{
    public class ValidateFilterAttribute : ActionFilterAttribute
    {
        //Bu method action çalıştırılırken çalışır.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Bizim context üzerinden gelen ModelState ile entegre yani biz fluent validation'u kullanmasak bile validation hatalarını görmek için bu modelstate'in IsValid üzerinden bir hata varmı yok mu kontrol edebiliriz.Fluent Validation'un güzelliği direk ModelState ile entegre geliyor.Serivce katmanında yazdığımız validationlar direk olarak ModelState'ye mapleniyor.Biz fluent validation da kullanmasak yine hatalar framework tarafından context'in ModelState'ine yükleniyor.
            // O zaman diyoruz ki IsValid değilse yani bir hata var ise if blokları arasında kendimiz döneriz.
            if (!context.ModelState.IsValid)
            {
                // Valuesle beraber bize modelstatedictionary döner bizim dictionary'e ihtiyacımız yok bu yüzden SelectMany ile beraber tek tek alıyoruz diyoruz ki artık x lambda ile girdiğimizde baştaki x artık bizim için bir modelstate entry oluyor.Bizim sınıfa da ihityacımız yok.Bize bu sınıftaki hatayı ver dememiz gerekiyor.Bunun için x.Errorrs yani bana hataları ver deriz.Ardın bize hataları verince baştaki errors değişkenine ModelError gelir.Artık elimizde bir hata sınıfı var.Ama bunun da daha tam string hatasını almadık.String hatasını aşağıdaki kodda bulunan selectten sonra ki kısımda alırız.
                // Selectten sonra demek isteğimiz ise bize gelen x'den sadece hata mesajlarını göster ve bunu da ToList'e çevir diyoruz.Daha derli toplu durması için tek satırda yaparız.  
                // Aşağıdaki SelectMany flat yapar düz yapar yani SelectMany den bize bir dictionary gelir ,bir liste gelir o liste de tek bir property'i almamıza imkan verir.Errors dan bize tek bir sınıf geliyor o sınıftan da hata mesajını ver diyoruz.
                var errors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x=>x.ErrorMessage).ToList();

                // BadRequestResult, response'nin body'sini boş göndeririz fakat biz responsenin body'sinde hata mesajlarıda göndermek istiyoruz.Bu yüzden object olanını seçeiyoruz.BadRequestObjectResult bunun için customresponsedto sınıfımızı döneriz.
                context.Result = new BadRequestObjectResult(CustomResponseDto<NoContentDto>.Fail(400, errors));

            }
        }
    }
}
