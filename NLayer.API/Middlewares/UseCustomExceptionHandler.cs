using Microsoft.AspNetCore.Diagnostics;
using NLayer.Core.DTOs;
using NLayer.Service.Exceptions;
using System.Text.Json;

namespace NLayer.API.Middlewares
{
    // Bir extension method yazabilmek için methodumuz mutlaka static olmalı.
    public static class UseCustomExceptionHandler
    {
        // Bu methodu hangi IApplicationBuilder için extension method yazarız.Program.cs deki app'in tipi web apllicationdur. Web application tipine f12 yapınca karşımıza IApplicationBuilderdan miras aldığı ortaya çıkar.Yani biz IApplicationBuilder'ı implemente eden tüm classlarda kullanabiliriz.Bu yüzden bu interface için bir extension method yazacağız.İsmi de aşağıda yazıyor
        public static void UseCustomException(this IApplicationBuilder app)
        {
            // Bu bir api uygulaması olduğu için filterdaki gibi otomatik bir model döner fakat biz kendi modelimizi döndürmek istiyoruz bu yüzden aşağıdaki kodları yazarız.
            // Bu bir api uygulaması olduğu için geriye bir json döneriz.
            app.UseExceptionHandler(config =>
            {
                // Run sonlandırıcı bir middlewaredir.Yani aşağıda yazdığımız koddan sonra akış geriye döner.Request buraya girdiği anda artık daha ileriye gitmeyecek controllerlara , methodlara kadar gitmeyecek burdan geriye döner.
                config.Run(async context =>
                {
                    // İlk önce contenttype'ını belirledik.
                    context.Response.ContentType = "application/json";
                    // Bir hata fırlatıldıysa bu middleware'e geldi o zaman hatayı alalım.
                    // IExceptionHandlerFeature bu interface üzerinden uygulamada fırlatılan hatayı alıyoruz yani bunun tipini aynı interface ile exceptiona da gideriz.
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    // Burda uygulama hata fırlatabilir bu yüzden burda 500 dönememiz lazım. Birde biz hata fırlatabilriz.Yani client'ın bir hatasından dolayı geriye bir hata fırlatmak isteyebiliriz. O zaman da 400 dönmemiz lazım. O zaman burda uygulama içerisinde kendi fırlatacağımız exceptionlarımız ayrı olması lazım ki burda ayırabilelim.
                    // Artık exceptionFeature'ın yanına nokta koyduktan sonra artık gelecek exception tipini ayırt edebiliriz.
                    var statusCode = exceptionFeature.Error switch
                    {
                        // Eğer bu hatanın tipi exception eğer clientside bir exception ise geriye 400 döner yani status coda 400 değeri ata.Eğer bunun dışında birşey ise 
                        ClientSideException => 400,
                        NotFoundException=>404,
                        // Eğer biz 500 hatası alıyorsak büyük ihtimal ile db'ye bağlanmak ile ilgili bir hata olabilir ve o hatayı clientlara dönmek çok uygun değil orda bir hata meydana geldi şeklinde genel bir hatamızı dönememiz uygun olacaktır.Bu yazdıklarım 500 durumu için geçerlidir.Çünkü bir db hatası aldığımızda bunu clientlara dönememizin bir mantığı yoktur.Orda o hatayı log'layıp burda serilog, nlog gibi kütüphaneler bütün exceptionları loglar. Eğer 500 ise geriye aşağıdaki statusCode kısmından sonra bir if bloğu yazıp kendimize ortak bir hata mesajı döneriz.Mesela işlem yapılırken bir hata meyda geldi gibi hatalar dönersek client bu hatanın server'da kaynaklı olduğunu anlar ve saçma sapan client'a da hata dönememiş oluruz.Ama mutlaka bu exceptionları loglamalıyız. 
                        _ => 500
                    };
                    // Artık status kodunu da belirtebiliriz.Çünkü eğer kendimiz bir hata fırlatıyorsak büyük ihtimalle clienttan dolayıdır.Mesela client id'si 5 olan datayı çekmek istiyordur ama id si 5 olan data yoktur.İşte orda bir hata fırlattığımız zaman buraya gelecek bakacak bizim tipimiz geriye 400 dönecek çünkü bu client'ın hatası. Id'si doğru olan id'si veritabanında var olan data göndermemiş o yüzden de 400 dönememiz lazım.
                    context.Response.StatusCode = statusCode;

                    // Burda bir response oluşturduk.Bizim bunu geriye dönememiz için Json'a seralize etmemiz lazım.
                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode, exceptionFeature.Error.Message);
                    // Eskiden Json çevirmek için NewtonSoft kullanıyorduk. Artık .net in kendi içerisinde de json'u serializer edebileceğimiz built-in bir sınıf geldi.Artık JsonSerializer sınıfını bult-in olarak kullanabiliriz.Extra bir kütüphanye gerek yok.
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                    // Biz product controller da direk tip dönüyoruz ve bir çevirme yapmadan direk Json dönüyor.Nedeni orda framework otomatik olarak bizim kullanmış olduğumuz tipten dolayı direk json a çevirir.Fakat burda artık kendimiz bir middleware yazdığımız için kendimiz dönüştürmemiz lazım yani controllerlarda geriye bir tip döndüğümüzde otomatik json'a dönüyor.Burda bir method yok burda artık kendimiz dönememiz lazım.

                    // Şimdi bu middleware'i program.cs de aktif hale getiremeliyiz.
                });
            });
        }
    }
}
