namespace NLayer.Core.Models
{
    // Abstract yazmamızın sebebi base entity den nesne örneği alınmasını önlemek 
    // Eğer herhangi bir referans tipimizi new anahtar sözcüğünü kullanmadan oluşturamıyorsak biz bunlara soyut nesneler deriz.Abstractlar soyut nesnelerdir çünkü new anahtar sözcüğünü kullanarak yeni bir nesne örneği oluşturamayız.Aynı şekilde Interfacelerde soyut yapılardır ve new anahtar sözcüğü ile oluşturamayız.
    // Abstract classlarımız projelerimizde classlarımızdaki ortak property ve methodları tanımladığımız yerlerdir.
    // Interfaceler de soyut nesnelerdir genellikle kontrat ve sözleşmelerimizi tanımladığımız yerlerdir.
    // Interfaceler'le yapabildiklerimizi abstract classlar ile de yapabiliriz çünkü ikisi de soyut yapıdır.Tek farkları Abstract class'ı başka bir class'a inherit ederken abstract class içine yazdığımız her bilgiyi almak zorunda değiliz fakat interfacelerde interfacein içine yazdığımız her bilgiyi interface kaltım aldığımız classda uygulamak zorundayız.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
