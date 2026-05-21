package Clases;

public class Dulce extends Producto {
    private String tipo; 

    public Dulce(String codigo, String nombre, double precio, int stock, String tipo) {
        super(codigo, nombre, precio, stock); 
        this.tipo = tipo;
   }
    public String getTipo() { return tipo; }
    @Override
    public String toString() {
        return super.toString() + " | Tipo: " + tipo;
    }
}
