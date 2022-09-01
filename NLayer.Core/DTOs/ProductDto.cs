
namespace NLayer.Core.DTOs
{
    public class ProductDto : BaseDto
    {
        // Clientlara bir dto dönerken bir updatedate dönememize gerek yoktur.
        // ProductDto' yu biz update yapacağımız bir endpointte kullanırsak bu base den bize created date geliyor. 
        // Client'ın bu created date'i görmesine gerek yoktur API'larımızdaki herbir endpointte özgü bir requestdto yine aynı endpointe özgü responsedto bu biraz masraflı olabilir yani her endpoint için kafadan 2 tane dto'muzun olması gerekir özelleştirilmiş 1-clienttan aldığımız dto endpointimiz için 2-clienta döneceğimiz dto endpoint için custom endpointler döneceksek onun dtolarını ayrı oluşturabiliriz.Örneğin MediaTR gibi bir kütüphane kullanıyorsak orada herbir endpoint için requestimiz ve response dtolarımızın mutlaka olması gerekiyor bu denge önemlidir biz burda her endpoint için tüm entitylerimizin crud operasyonlarındaki herbir endpoint için request veya response dtolar oluşturmak biraz fazla sınıf oluşturmaya neden olabilir bu yüzden mümkün olduğunca ortak kullanmaya çalışmalıyız.

        // Name alanı illa gerekliyse eğer aşağıdaki required attributunu name üzerinde kullanırız.Required olan yer doldurulmadan geçince verilecek mesaj'ı da yazabiliyoruz.
        //[Required(ErrorMessage ="Name alanını doldurun")]
        public string Name { get; set; }

        // Stoğun belirli aralıklarda olmasını istiyorsak range kullanırız.Requireddaki gibi error mesajı yazabiliriz.Fakat bu range ve required'ın kullanım şekilleri doğru değildir.Bu kullanımlardan mümkün olduğunca kaçınmalıyız.Validationları tamamen ayrı bir yerde gerçekleştirmeliyiz.Nedeni büyük projelerdeki yönetimi zor olacağından dolayı.
        //[Range(1,100)]
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
