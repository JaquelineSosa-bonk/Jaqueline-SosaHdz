package Clases;

@SuppressWarnings("serial")
public class InventarioException extends Exception {
    public InventarioException(String mensaje) {
        super(mensaje);
        System.err.println("Alerta del sistema" + mensaje + "");
        try { Thread.sleep(200); } catch (InterruptedException e) {} 
    }
}
