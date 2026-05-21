namespace Clases
{

    public class Dulce : Producto
    {
        private string tipo;

        public Dulce(string codigo, string nombre, double precio, int stock, string tipo)
            : base(codigo, nombre, precio, stock)
        {
            this.tipo = tipo;
        }

        public string GetTipo() { return tipo; }

        public override string ToString()
        {
           
            return base.ToString() + " | Tipo: " + tipo;
        }
    }
}