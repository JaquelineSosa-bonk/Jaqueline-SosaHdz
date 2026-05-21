package Clases;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class Venta {
    private String folio;
    private String fecha;
    private double total;
    private String detalles;

    public Venta(String folio, double total, String detalles) {
        this.folio = folio;
        this.total = total;
        this.detalles = detalles;

        LocalDateTime ahora = LocalDateTime.now();
        DateTimeFormatter formato = DateTimeFormatter.ofPattern("dd/MM/yyyy HH:mm:ss");
        this.fecha = ahora.format(formato);
    }

    public String getFolio() { return folio; }
    public String getFecha() { return fecha; }
    public double getTotal() { return total; }
    public String getDetalles() { return detalles; }

    public String aLineaTxt() {
        return folio + "|" + fecha + "|" + total + "|" + detalles;
    }
}