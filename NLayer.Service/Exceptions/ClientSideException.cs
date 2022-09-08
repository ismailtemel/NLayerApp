namespace NLayer.Service.Exceptions
{
    public class ClientSideException : Exception
    {
        // Constructorun da exceptiona mesaj göndeririz.Aşağıda base dediğimiz exception'un constructoruna gidiyor.
        public ClientSideException(string message) : base(message)
        {

        }
    }
}
