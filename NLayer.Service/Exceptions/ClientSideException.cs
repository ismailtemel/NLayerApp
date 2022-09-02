using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Exceptions
{
    public class ClientSideException : Exception
    {
        // Constructorun da exceptiona mesaj göndeririz.Aşağıda base dediğimiz exception'un constructoruna gidiyor.
        public ClientSideException(string message):base(message)
        {

        }
    }
}
