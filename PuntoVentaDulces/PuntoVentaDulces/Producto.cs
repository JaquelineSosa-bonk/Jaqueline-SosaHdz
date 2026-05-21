namespace Clases
{
    public abstract class Producto
    {
        protected string codigo;
        protected string nombre;
        protected double precio;
        protected int stock;

        public Producto(string codigo, string nombre, double precio, int stock)
        {
            this.codigo = codigo;
            this.nombre = nombre;
            this.precio = precio;
            this.stock = stock;
        }
        public string GetCodigo() { return codigo; }
        public string GetNombre() { return nombre; }
        public double GetPrecio() { return precio; }
        public int GetStock() { return stock; }

        public void SetPrecio(double precio) { this.precio = precio; }
        public void SetStock(int stock) { this.stock = stock; }

        public override string ToString()
        {

            return $"[{codigo}] {nombre,-15} | Precio: ${precio:0.00} | Stock: {stock}";
        }
    }
}