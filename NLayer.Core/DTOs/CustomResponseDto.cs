using System.Text.Json.Serialization;

namespace NLayer.Core.DTOs
{
    public class CustomNoContentResponseDto
    {
        // Geriye bir data dönmeyeceğimiz zaman bu class , geriye birşey döneceğimiz zaman diğer classı kullanırız.

        [JsonIgnore]
        public int StatusCode { get; set; }
        public List<String> Errors { get; set; }

        public static CustomNoContentResponseDto Success(int statusCode)
        {
            return new CustomNoContentResponseDto { StatusCode = statusCode };
        }
        public static CustomNoContentResponseDto Fail(int statusCode, List<string> errors)
        {
            return new CustomNoContentResponseDto { StatusCode = statusCode, Errors = errors };
        }
        // Aşağıdaki methoddu da 1 tane hata gelince kullanmak için yazarız.
        // Bu yüzden direk error değil de Errors = new List<string> { error } bu şekilde tanımlarız.
        // Sonuçta 1 tane de hata dönsek mutlaka bir dizin içerisinde dönüyor olacağız.
        public static CustomNoContentResponseDto Fail(int statusCode, string error)
        {
            return new CustomNoContentResponseDto { StatusCode = statusCode, Errors = new List<string> { error } };
        }
    }
    public class CustomResponseDto<T>
    {
        // Bazen aşağıdaki T dataya biz null data gönderebiliriz dikkat edersek eğer aşağıdaki methodlarda yazıdğımız statusCode'u  gönderdiğimizde T data null olur.
        // Bazen de aşağıdaki datayı göndermeyeceksek eğer responseda boş bir class oluştururuz ve farklı farklı implemente edebiliriz.İstersek null göndeririz istersek T data almayan bir  customResponsemiz olabilir veya farketmez aynı propertyleri dönüyorsak class'ımızın isminin bir anlamı yoktur.
        // Dto klasöründe 1 tane daha class oluştururuz bu da boş bir dto olacak sadece geriye bir data dönmek istemediğimizde açık açık belirtmek için. Null yazmaktansa daha güzel bir     isim dönebiliriz.Ama zorunlu değil isteğe göre null da dönebiliriz.İstersek de sadece data ve error propertylerimizi alan ayrı classlarımız da olabilir.Ama yeterki ortak model   olsun. Farklı farklı sınıflarımızda farklı farklı propertyler dönemeyiz.Oluşturduğumuz farklı classlar ortak modelde olduğu için hangi classı dönersek yine ikiside json a        dönüştürürleceği için birşey farketmez fakat property isimleri aynı olmalı. 
        public T Data { get; set; }
        // Biz aşağıda yazdığımız status kodu dış dünayaya açmak istemiyoruz yani bu endpointe istek yapan clientlara status kod dönmek istemiyoruz zaten clientlar bir istek yaptığında  status kodu elde ediyorlar.Biz bir endpoint'e istek yaptığımızda geriye mutlaka bir durum kodu almak zorundayız. Yani illaki döneceğimiz response'ın body'sinde status kodu      dönememize gerek yok ama bu bana kod içerisinde lazım bu yüzden bunu JsonIgnore ile jsona dönüştürürken ignore et.Aşğıdaki status kod asla clientlara dönmez çünkü zaten      onlar bir istek yaptıklarında görecekler.
        [JsonIgnore]
        public int StatusCode { get; set; }
        public List<String> Errors { get; set; }

        // Herhangi bir classın içerisinde statik ve geriye yeni instance dönen methodlar varsa bunlara statik factory method denir.Aslında factor design pattern'dan gelir .Factor design pattern ayrı classlar veya ayri interfaceler oluşturmak yerine direk hangi sınıfı dönmek istiyorsak o sınıfın içerisinde statik methodlar tanımlayarak geriye instanceler döneriz yani new anahtar sözcüğünü kullanmak yerine direk aşağıdaki methodları kullanarak nesne üretme olayını aynı sınıf içerisinde gerçekleştiririz.Buna statik factory method veya static factory method design pattern olarak da geçer. Factory method design pattern da biraz daha implementasyon için interface ve classlar oluşturmamıza gerek varken bu statik factory methodlarda ise hiç ayrı interface ve factory method oluşturmamıza gerek yok direk olarak sınıf içerisinde nesne örnekleri dönebiliriz.Bu sayede nesne oluşturma işlemini kontrol altına alırız.Builder design pattern,Factory design pattern,Abstract factory design pattern,prototype gibi bu design patternların hep amacı nesne üretme olayını soyutlamak nesne üretme olayını clientlardan almak tamamen ayrı bir yerde oluşturmak
        // New anahtar kelimesini kullanmamak için statik methodlar oluştururuz.Aşağıdaki gibi success olduğunda dönsün fail olduğunda dönsün vs şeklinde
        public static CustomResponseDto<T> Success(int statusCode, T data)
        {
            return new CustomResponseDto<T> { Data = data, StatusCode = statusCode };
        }
        // Errors = null default referans tipi olduğu için doldurmamaıza gerek yoktur
        // Bir tane daha success olabilir nedeni bazen geriye başarılı geri döneriz ama illaki data dönemyiz örneğin productupdate olan bir endpoit düşünürüz geriye birşey dönmek zorunda değiliz update esnasında bu yüzden yukarıdaki datayı da doldurmamıza gerek yok sadece durum kodu veririz.
        public static CustomResponseDto<T> Success(int statusCode)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode };
        }
        public static CustomResponseDto<T> Fail(int statusCode, List<string> errors)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = errors };
        }
        // Aşağıdaki methoddu da 1 tane hata gelince kullanmak için yazarız.
        // Bu yüzden direk error değil de Errors = new List<string> { error } bu şekilde tanımlarız.
        // Sonuçta 1 tane de hata dönsek mutalka bir dizin içerisinde dönüyor olacağız.
        public static CustomResponseDto<T> Fail(int statusCode, string error)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = new List<string> { error } };
        }
    }
}
