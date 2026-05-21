using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Clases
{
    public class Program
    {
        private static int contadorVentas = 1;

        public static void Main(string[] args)
        {
            GestorInventario<Dulce> inventario = new GestorInventario<Dulce>(
                "inventario_dulceria.txt",
                linea =>
                {
                    string[] p = linea.Split('|');
                    if (p.Length == 5)
                    {
                        return new Dulce(p[0], p[1], double.Parse(p[2]), int.Parse(p[3]), p[4]);
                    }
                    return null;
                },
                d => $"{d.GetCodigo()}|{d.GetNombre()}|{d.GetPrecio()}|{d.GetStock()}|{d.GetTipo()}"
            );

            bool corriendo = true;

            while (corriendo)
            {
                try
                {
                    Console.WriteLine("");
                    Console.WriteLine("Dulceria Sosa");
                    Console.WriteLine("1. Entrar en MODO ADMINISTRADOR");
                    Console.WriteLine("2. Entrar en MODO CLIENTE (Comprar)");
                    Console.WriteLine("3. Salir del sistema");
                    Console.Write("Elige tu modo de acceso: ");
                    Console.WriteLine("");

                    int modo = int.Parse(Console.ReadLine());

                    switch (modo)
                    {
                        case 1:
                            MenuAdmin(inventario);
                            break;
                        case 2:
                            MenuCliente(inventario);
                            break;
                        case 3:
                            Console.WriteLine("Apagando sistema...");
                            corriendo = false;
                            break;
                        default:
                            Console.Error.WriteLine("Opción no válida. Usa 1, 2 o 3.");
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.Error.WriteLine("ERROR: Escribiste letras o símbolos. Solo usa números.");
                }
            }
        }

        private static void MenuAdmin(GestorInventario<Dulce> inv)
        {
            bool enAdmin = true;
            while (enAdmin)
            {
                try
                {
                    Console.WriteLine("Administracion");
                    Console.WriteLine("1. Ver todos los dulces");
                    Console.WriteLine("2. Añadir nuevo dulce");
                    Console.WriteLine("3. Modificar precio/stock de un dulce");
                    Console.WriteLine("4. Eliminar un dulce");
                    Console.WriteLine("5. Volver al Menú Principal");
                    Console.WriteLine("6. Ver Corte de Caja ");
                    Console.Write("¿Qué deseas hacer?: ");
                    Console.WriteLine("");

                    int op = int.Parse(Console.ReadLine());

                    if (op == 1)
                    {
                        MostrarInventario(inv);
                    }
                    else if (op == 2)
                    {
                        Console.Write("Código: "); string cod = Console.ReadLine();
                        Console.Write("Nombre: "); string nom = Console.ReadLine();
                        Console.Write("Precio: "); double pre = double.Parse(Console.ReadLine());
                        Console.Write("Stock inicial: "); int stk = int.Parse(Console.ReadLine());
                        Console.Write("Tipo: "); string tip = Console.ReadLine(); // Le añadí los dos puntos para que se vea parejo

                        inv.Agregar(new Dulce(cod, nom, pre, stk, tip));
                        Console.WriteLine("Dulce añadido al catálogo");
                    }
                    else if (op == 3)
                    {
                        MostrarInventario(inv);
                        Console.Write("Escribe el código del dulce a modificar: ");
                        Dulce d = inv.BuscarPorCodigo(Console.ReadLine());

                        Console.Write("Nuevo precio (Actual: " + d.GetPrecio() + "): $");
                        d.SetPrecio(double.Parse(Console.ReadLine()));

                        Console.Write("Añadir stock (Actual: " + d.GetStock() + "): ");
                        d.SetStock(d.GetStock() + int.Parse(Console.ReadLine()));

                        inv.SincronizarCambios();
                        Console.WriteLine("Dulce actualizado");
                    }
                    else if (op == 4)
                    {
                        MostrarInventario(inv);
                        Console.Write("Escribe el código del dulce a eliminar: ");
                        Dulce d = inv.BuscarPorCodigo(Console.ReadLine());
                        inv.Eliminar(d);
                        Console.WriteLine("Dulce eliminado");
                    }
                    else if (op == 5)
                    {
                        enAdmin = false;
                    }
                    else if (op == 6)
                    {
                        MostrarCorteDeCaja();
                    }
                    else
                    {
                        Console.Error.WriteLine("Opción fuera de rango.");
                    }
                }
                catch (FormatException)
                {
                    Console.Error.WriteLine("ERROR: Ingresa números, no letras.");
                }
                catch (InventarioException)
                {
                }
            }
        }

        private static void MenuCliente(GestorInventario<Dulce> inv)
        {
            List<Dulce> carritoDulces = new List<Dulce>();
            List<int> carritoCantidades = new List<int>();
            bool comprando = true;
            if (inv.ObtenerTodos().Count == 0)
            {
                Console.Error.WriteLine("La tienda está vacía ahorita");
                return;
            }

            while (comprando)
            {
                try
                {
                    Console.WriteLine("");
                    MostrarInventario(inv);
                    Console.WriteLine("Escribe el 'Código' del dulce para llevar (o '0' para ir a pagar):");
                    string cod = Console.ReadLine();

                    if (cod == "0")
                    {
                        comprando = false;
                    }
                    else
                    {
                        Dulce d = inv.BuscarPorCodigo(cod);
                        Console.Write("¿Cuántas piezas de " + d.GetNombre() + " quieres?: ");
                        int cantidad = int.Parse(Console.ReadLine());

                        if (cantidad <= 0)
                        {
                            Console.Error.WriteLine("Mete una cantidad válida.");
                        }
                        else if (cantidad > d.GetStock())
                        {
                            throw new InventarioException("Solo tenemos " + d.GetStock() + " piezas de " + d.GetNombre());
                        }
                        else
                        {
                            d.SetStock(d.GetStock() - cantidad);
                            inv.SincronizarCambios();

                            carritoDulces.Add(d);
                            carritoCantidades.Add(cantidad);
                            Console.WriteLine("Agregado al carrito");
                        }
                    }
                }
                catch (FormatException)
                {
                    Console.Error.WriteLine("ERROR: La cantidad debe ser un número entero.");
                }
                catch (InventarioException)
                {
                }
            }

            if (carritoDulces.Count > 0)
            {
                double total = 0;
                StringBuilder detallesParaTxt = new StringBuilder();

                Console.WriteLine("----------------------------------");
                Console.WriteLine("        TICKET DE COMPRA          ");
                Console.WriteLine("----------------------------------");
                for (int i = 0; i < carritoDulces.Count; i++)
                {
                    Dulce d = carritoDulces[i];
                    int cant = carritoCantidades[i];
                    double subtotal = d.GetPrecio() * cant;
                    total += subtotal;

                    Console.WriteLine($"{cant} x {d.GetNombre(),-15} = ${subtotal:0.00}");
                    detallesParaTxt.Append(cant).Append("x ").Append(d.GetNombre()).Append(", ");
                }
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"TOTAL A PAGAR:          ${total:0.00}");
                Console.WriteLine("¡Gracias por tu compra en Dulcería Sosa!");
                Console.WriteLine("");

                string folio = "V-" + contadorVentas++;
                Venta nuevaVenta = new Venta(folio, total, detallesParaTxt.ToString());
                RegistrarVentaTxt(nuevaVenta);
            }
        }

        private static void MostrarInventario(GestorInventario<Dulce> inv)
        {
            Console.WriteLine("Inventario Disponible");
            if (inv.ObtenerTodos().Count == 0)
            {
                Console.WriteLine("No hay productos registrados aún.");
            }
            else
            {
                foreach (Dulce d in inv.ObtenerTodos())
                {
                    Console.WriteLine(d.ToString());
                }
            }
            Console.WriteLine("-----------------------------");
        }

        private static void RegistrarVentaTxt(Venta v)
        {
            try
            {
                using (StreamWriter pw = new StreamWriter("historial_ventas.txt", true))
                {
                    pw.WriteLine(v.ALineaTxt());
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error al guardar la venta en el historial.");
            }
        }

        private static void MostrarCorteDeCaja()
        {
            if (!File.Exists("historial_ventas.txt"))
            {
                Console.WriteLine("Aún no hay ventas registradas. La caja está en $0.00.");
                return;
            }

            double totalCaja = 0;
            Console.WriteLine("             CORTE DE CAJA");
            Console.WriteLine($"{"FOLIO",-6} | {"FECHA/HORA",-20} | {"TOTAL",-10} | ARTÍCULOS");
            Console.WriteLine("------------------------------------------------------------------");

            try
            {
                using (StreamReader br = new StreamReader("historial_ventas.txt"))
                {
                    string linea;
                    while ((linea = br.ReadLine()) != null)
                    {
                        string[] partes = linea.Split('|');
                        if (partes.Length == 4)
                        {
                            double subtotal = double.Parse(partes[2]);
                            Console.WriteLine($"{partes[0],-6} | {partes[1],-20} | ${subtotal,-9:0.00} | {partes[3]}");
                            totalCaja += subtotal;
                        }
                    }
                }
                Console.WriteLine("");
                Console.WriteLine($"TOTAL INGRESOS DEL DÍA: ${totalCaja:0.00}");
                Console.WriteLine("");
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error al leer el historial de ventas.");
            }
        }
    }
}