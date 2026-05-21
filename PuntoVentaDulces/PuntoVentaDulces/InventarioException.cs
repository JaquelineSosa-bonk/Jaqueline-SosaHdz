using System;
using System.Threading;

namespace Clases
{
    public class InventarioException : Exception
    {

        public InventarioException(string mensaje) : base(mensaje)
        {
            Console.Error.WriteLine("Alerta del sistema" + mensaje);
            Thread.Sleep(200);
        }
    }
}