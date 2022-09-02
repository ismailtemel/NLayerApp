using FluentValidation;
using NLayer.Core.DTOs;

namespace NLayer.Service.Validation
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            // Bu validator'u aktif etmeden önce bir hata alırız.Hata şudur, productdto'ya api'den biz sadece name gönderiyoruz fakat categoryId 0 aldı name'i verdi ama price ve stoğu 0 geldi.CategoryId 0 olamaz çünkü db de 0 diye bir kategori tablosu yok bu yüzden uygulama patlar.(ProductController Save endpointine göre açıklama yapıldı.)Bu yüzden illaki bir aralık belirtmemiz lazım value typelar için.Referans tipler için default olarak birşey belirtmezsek null olur.
            //Validation yazarken rulefor diye generic bir method kullanırız.
            // Aşağıdaki kodun anlamı ilk önce name'i aldık ardından null olmaması için notnull methodunu ekledik.Eğer null olursa bize vereceği mesajı withmessage methodu ile yazdırırız.Name propertysinin ismini fluent validation da bir placeholder ile almamıza imkan veriyor bu da PropertyName'i yazdığımızda bu placeholder'a fluent validation name'i direk propertyname ksımına getiriyor.
            RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} is required");

            // Price alanı normalde bir decimal'dır.Bunun default'u 0'dır. Yani herhangi bir client yeni bir productdto kaydederken price property'sini göndermese dahi bizim endpointimize değeri 0 olarak gelir.
            // Aynı durum stock propertysi için de geçerlidir.O zaman notnull,notempty bir işe yaramaz bizim o zaman kesin bir aralık belirtmemiz gerekiyor. Örnek olarak price alanı örneğin boolean tipinin defaultu false'dur yani özellikle value tiplerinde default değerleri vardır ve bu default değerleri olduğu için de biz notull ile kontrol edemeyiz.Peki string de neden var ? String referans tipli olduğu için null olabilir.Ama double , datetime bunlar value type olduğu için default değerleri var bu yüzden biz notnull,notempty kullanmamız uygun olmaz.Bu yüzden aşağıda mutlaka inclusivebetween adında bir method kullanırız.Yani dahil edeceğimiz aralığı belirteceğimiz bir methoddur.
            RuleFor(x => x.Price).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
            RuleFor(x => x.Stock).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
            RuleFor(x => x.CategoryId).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");


        }
    }
}
