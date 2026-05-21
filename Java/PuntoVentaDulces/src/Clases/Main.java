package Clases;

import java.io.*;
import java.util.ArrayList;
import java.util.Scanner;

public class Main {
    private static int contadorVentas = 1; 

    public static void main(String[] args) {
        Scanner entrada = new Scanner(System.in);
        
        GestorInventario<Dulce> inventario = new GestorInventario<>(
            "inventario_dulceria.txt",
            linea -> {
                String[] p = linea.split("\\|");
                if (p.length == 5) {
                    return new Dulce(p[0], p[1], Double.parseDouble(p[2]), Integer.parseInt(p[3]), p[4]);
                }
                return null;
            },
            d -> d.getCodigo() + "|" + d.getNombre() + "|" + d.getPrecio() + "|" + d.getStock() + "|" + d.getTipo()
        );

        boolean corriendo = true;

        while (corriendo) {
            try {
            	System.out.println("");
                System.out.println("Dulceria Sosa");
                System.out.println("1. Entrar en MODO ADMINISTRADOR");
                System.out.println("2. Entrar en MODO CLIENTE (Comprar)");
                System.out.println("3. Salir del sistema");
                System.out.print("Elige tu modo de acceso: ");
                System.out.println("");
                
                int modo = Integer.parseInt(entrada.nextLine());

                switch (modo) {
                    case 1:
                        menuAdmin(inventario, entrada);
                        break;
                    case 2:
                        menuCliente(inventario, entrada);
                        break;
                    case 3:
                        System.out.println("Apagando sistema...");
                        corriendo = false;
                        break;
                    default:
                        System.err.println("Opción no válida. Usa 1, 2 o 3.");
                }
            } catch (NumberFormatException e) {
                System.err.println("ERROR: Escribiste letras o símbolos. Solo usa números.");
            }
        }
        entrada.close();
    }

    private static void menuAdmin(GestorInventario<Dulce> inv, Scanner sc) {
        boolean enAdmin = true;
        while (enAdmin) {
            try {
                System.out.println("Administracion");
                System.out.println("1. Ver todos los dulces");
                System.out.println("2. Añadir nuevo dulce");
                System.out.println("3. Modificar precio/stock de un dulce");
                System.out.println("4. Eliminar un dulce");
                System.out.println("5. Volver al Menú Principal");
                System.out.println("6. Ver Corte de Caja ");
                System.out.print("¿Qué deseas hacer?: ");
                System.out.println("");
                
                int op = Integer.parseInt(sc.nextLine());

                if (op == 1) {
                    mostrarInventario(inv);
                } 
                else if (op == 2) {
                    System.out.print("Código: "); String cod = sc.nextLine();
                    System.out.print("Nombre: "); String nom = sc.nextLine();
                    System.out.print("Precio: "); double pre = Double.parseDouble(sc.nextLine());
                    System.out.print("Stock inicial: "); int stk = Integer.parseInt(sc.nextLine());
                    System.out.print("Tipo"); String tip = sc.nextLine();
                    
                    inv.agregar(new Dulce(cod, nom, pre, stk, tip));
                    System.out.println("Dulce añadido al catálogo");
                } 
                else if (op == 3) {
                    mostrarInventario(inv);
                    System.out.print("Escribe el código del dulce a modificar: ");
                    Dulce d = inv.buscarPorCodigo(sc.nextLine());
                    
                    System.out.print("Nuevo precio (Actual: " + d.getPrecio() + "): $");
                    d.setPrecio(Double.parseDouble(sc.nextLine()));
                    
                    System.out.print("Añadir stock (Actual: " + d.getStock() + "): ");
                    d.setStock(d.getStock() + Integer.parseInt(sc.nextLine()));
                    
                    inv.sincronizarCambios();
                    System.out.println("Dulce actualizado");
                } 
                else if (op == 4) {
                    mostrarInventario(inv);
                    System.out.print("Escribe el código del dulce a eliminar: ");
                    Dulce d = inv.buscarPorCodigo(sc.nextLine());
                    inv.eliminar(d);
                    System.out.println("Dulce eliminado");
                } 
                else if (op == 5) {
                    enAdmin = false;
                } 
                else if (op == 6) {
                    mostrarCorteDeCaja();
                } else {
                    System.err.println("Opción fuera de rango.");
                }
            } catch (NumberFormatException e) {
                System.err.println("ERROR: Ingresa números, no letras.");
            } catch (InventarioException e) {

            }
        }
    }

    private static void menuCliente(GestorInventario<Dulce> inv, Scanner sc) {
        ArrayList<Dulce> carritoDulces = new ArrayList<>();
        ArrayList<Integer> carritoCantidades = new ArrayList<>();
        boolean comprando = true;

        if (inv.obtenerTodos().isEmpty()) {
            System.err.println("La tienda está vacía ahorita");
            return;
        }

        while (comprando) {
            try {
                System.out.println("");
                mostrarInventario(inv);
                System.out.println("Escribe el 'Código' del dulce para llevar (o '0' para ir a pagar):");
                String cod = sc.nextLine();

                if (cod.equals("0")) {
                    comprando = false;
                } else {
                    Dulce d = inv.buscarPorCodigo(cod);
                    System.out.print("¿Cuántas piezas de " + d.getNombre() + " quieres?: ");
                    int cantidad = Integer.parseInt(sc.nextLine());

                    if (cantidad <= 0) {
                        System.err.println("Mete una cantidad válida.");
                    } else if (cantidad > d.getStock()) {
                        throw new InventarioException("Solo tenemos " + d.getStock() + " piezas de " + d.getNombre());
                    } else {
                        d.setStock(d.getStock() - cantidad);
                        inv.sincronizarCambios();
                        
                        carritoDulces.add(d);
                        carritoCantidades.add(cantidad);
                        System.out.println("Agregado al carrito");
                    }
                }
            } catch (NumberFormatException e) {
                System.err.println("ERROR: La cantidad debe ser un número entero.");
            } catch (InventarioException e) {
            }
        }

        if (!carritoDulces.isEmpty()) {
            double total = 0;
            StringBuilder detallesParaTxt = new StringBuilder();

            System.out.println("----------------------------------");
            System.out.println("        TICKET DE COMPRA          ");
            System.out.println("----------------------------------");
            for (int i = 0; i < carritoDulces.size(); i++) {
                Dulce d = carritoDulces.get(i);
                int cant = carritoCantidades.get(i);
                double subtotal = d.getPrecio() * cant;
                total += subtotal;
                
                System.out.printf("%d x %-15s = $%.2f\n", cant, d.getNombre(), subtotal);
                detallesParaTxt.append(cant).append("x ").append(d.getNombre()).append(", ");
            }
            System.out.println("----------------------------------");
            System.out.printf("TOTAL A PAGAR:          $%.2f\n", total);
            System.out.println("¡Gracias por tu compra en Dulcería Sosa!");
            System.out.println("");

            String folio = "V-" + contadorVentas++;
            Venta nuevaVenta = new Venta(folio, total, detallesParaTxt.toString());
            registrarVentaTxt(nuevaVenta);
        }
    }


    private static void mostrarInventario(GestorInventario<Dulce> inv) {
        System.out.println("Inventario Disponible");
        if (inv.obtenerTodos().isEmpty()) {
            System.out.println("No hay productos registrados aún.");
        } else {
            for (Dulce d : inv.obtenerTodos()) {
                System.out.println(d.toString());
            }
        }
        System.out.println("-----------------------------");
    }

    private static void registrarVentaTxt(Venta v) {
        try (PrintWriter pw = new PrintWriter(new FileWriter("historial_ventas.txt", true))) {
            pw.println(v.aLineaTxt());
        } catch (Exception e) {
            System.err.println("Error al guardar la venta en el historial.");
        }
    }

    private static void mostrarCorteDeCaja() {
        File archivo = new File("historial_ventas.txt");
        if (!archivo.exists()) {
            System.out.println("Aún no hay ventas registradas. La caja está en $0.00.");
            return;
        }

        double totalCaja = 0;
        System.out.println("             CORTE DE CAJA");
        System.out.printf("%-6s | %-20s | %-10s | %s\n", "FOLIO", "FECHA/HORA", "TOTAL", "ARTÍCULOS");
        System.out.println("------------------------------------------------------------------");

        try (BufferedReader br = new BufferedReader(new FileReader(archivo))) {
            String linea;
            while ((linea = br.readLine()) != null) {
                String[] partes = linea.split("\\|");
                if (partes.length == 4) {
                    System.out.printf("%-6s | %-20s | $%-9.2f | %s\n", partes[0], partes[1], Double.parseDouble(partes[2]), partes[3]);
                    totalCaja += Double.parseDouble(partes[2]);
                }
            }
            System.out.println("");
            System.out.printf("TOTAL INGRESOS DEL DÍA: $%.2f\n", totalCaja);
            System.out.println("");
        } catch (Exception e) {
            System.err.println("Error al leer el historial de ventas.");
        }
    }
}