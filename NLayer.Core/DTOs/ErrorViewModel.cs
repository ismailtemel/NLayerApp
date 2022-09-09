namespace NLayer.Core.DTOs
{
    // Bizim burda default olarak gelen bir error sayfamız var
    public class ErrorViewModel
    {
        // Error'un default bie list'ini oluşturmadığımız için null hatası aldık.
        // Aşağıdaki listeyi oluşturmadığımız gibi notfoundfilter'da eklemeye çalışıyoruz ve eklemeye çalışırken null hatası alıyoruz.
        public List<string> Errors { get; set; } = new List<string>();
    }
}
