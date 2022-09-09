﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.Web
{
    // Burda dinamik olarak okuyabiliriz .Burda dinamik bir generic T alırız.
    public class NotFoundFilter<T> : IAsyncActionFilter where T : BaseEntity
    {
        // Filter'ımız bir tip alıyorsa onu Program.cs dosyasına ekleriz.
        private readonly IService<T> _service;

        public NotFoundFilter(IService<T> service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //ProductController'daki GetById methodunda daha GetById'ye gelmeden bizim yazacağımız filter GetById'de parametre olarak bulunan (int id) yandaki id'ye sahip mi değil mi kontrol edecek.O zaman bizim ilgili endpointteki id'yi yakalamamız lazım.Id'yi aşağıdaki gibi yakalarız.
            // Aşağıdaki context bizim uygulamamızın kalbidir.Context'ten bütün request ve responselere erişebiliriz.
            // Context ile actionarguments property'sine ulaştık ardından bu yazdığımız property üzerinden values'larına git diyoruz burdan da bize ilk olanı al diyoruz.Yani paramtredeki id'yi.
            var idValue = context.ActionArguments.Values.FirstOrDefault();

            // Yukarıdaki işlemleri yaptıktan sonra şimdi null mı yokmu onu kontrol ederiz.
            if (idValue == null)
            {
                // Eğer id nullsa yoluna devam et deriz.Yani demekki bana bir id gelmiyorsa demekki biz bu id'ye sahip birşeyle karşılaştırmamıza gerek yok.O zaman diyoruz ki biz burda await ile beraber next sınıfının invoke methodunu çağıracağız.Yani yoluna devam et diyoruz.Döngünün içinden çıkılmasına gerek olmadığı için return'u aşağıdaki kod satırının altına yazarız ve döneriz.
                await next.Invoke();
                return;
            }
            // Eğer id'si varsa value(değer)'u id ye cast ederiz yani dönüştürürüz.
            // Artık elimizde ne var productcontroller'daki id var.Bu id controller'ın da id'si olabilir çünkü bu class'da generic ifade kulanıyoruz.
            var id = (int)idValue;

            // AnyEntity ile entity varmı yokmu kontrol ederiz.await ile beraber bizim kontrol edebilmemiz için bir servis katmanına ihtiyacımız var o zaman bu classın en üstüne service'i ekleriz. Ardından aşağıdaki gibi id'nin olup olmadığını kontrol ederiz.Eğer ihtiyacımız olursa repo üzerinden de gidebiliriz.Biz şimdi id üzerinden gidiyoruz.Id varmı yokmu kontrol eder.Aşağıda getbyid yerine anyasync de kullanabiliriz fakat bu sefer id ye ulaşamıyoruz.Bunun çözümü için yukarıda class olarak belirttiğimiz yeri core katmanında bulunan kendi yazdığımız baseentity ile ulaşabiliriz.Eğer aşağıdaki koşul sağlanıyorsa yoluna devam eder ama else ise 
            var anyEntity = await _service.AnyAsync(x => x.Id == id);

            // Eğer entity varsa 
            if (anyEntity)
            {
                await next.Invoke();
                return;
            }

            var errorViewModel = new ErrorViewModel();
            errorViewModel.Errors.Add($"{typeof(T).Name}({id}) not found");

            context.Result = new RedirectToActionResult("Error", "Home", errorViewModel);
        }
    }
}
