using System;
namespace Clases
{
    public class Venta
    {
        private string folio;
        private string fecha;
        private double total;
        private string detalles;

        public Venta(string folio, double total, string detalles)
        {
            this.folio = folio;
            this.total = total;
            this.detalles = detalles;

            DateTime ahora = DateTime.Now;
            this.fecha = ahora.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public string GetFolio() { return folio; }
        public string GetFecha() { return fecha; }
        public double GetTotal() { return total; }
        public string GetDetalles() { return detalles; }

        public string ALineaTxt()
        {
            return folio + "|" + fecha + "|" + total + "|" + detalles;
        }
    }
}