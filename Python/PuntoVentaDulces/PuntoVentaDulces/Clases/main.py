# -*- coding: utf-8 -*-
import os
import sys
from dulce import Dulce
from venta import Venta
from gestor_inventario import GestorInventario
from inventario_exception import InventarioException

contador_ventas = 1

def desde_linea(linea):
    p = linea.split('|')
    if len(p) == 5:
        return Dulce(p[0], p[1], float(p[2]), int(p[3]), p[4])
    return None

def a_linea(d):
    return f"{d.get_codigo()}|{d.get_nombre()}|{d.get_precio()}|{d.get_stock()}|{d.get_tipo()}"

def mostrar_inventario(inv):
    print("Inventario Disponible")
    if not inv.obtener_todos():
        print("No hay productos registrados aun.")
    else:
        for d in inv.obtener_todos():
            print(d)
    print("-----------------------------")

def registrar_venta_txt(v):
    try:
        with open("historial_ventas.txt", "a", encoding="utf-8") as f:
            f.write(v.a_linea_txt() + "\n")
    except Exception:
        print("Error al guardar la venta en el historial.", file=sys.stderr)

def mostrar_corte_de_caja():
    if not os.path.exists("historial_ventas.txt"):
        print("Aun no hay ventas registradas. La caja esta en $0.00.")
        return

    total_caja = 0.0
    print("             CORTE DE CAJA")
    print(f"{'FOLIO':<6} | {'FECHA/HORA':<20} | {'TOTAL':<10} | ARTICULOS")
    print("-" * 66)

    try:
        with open("historial_ventas.txt", "r", encoding="utf-8") as f:
            for linea in f:
                p = linea.strip().split('|')
                if len(p) == 4:
                    total = float(p[2])
                    print(f"{p[0]:<6} | {p[1]:<20} | ${total:<9.2f} | {p[3]}")
                    total_caja += total
        print("")
        print(f"TOTAL INGRESOS DEL DIA: ${total_caja:.2f}")
        print("")
    except Exception:
        print("Error al leer el historial de ventas.", file=sys.stderr)

def menu_admin(inv):
    en_admin = True
    while en_admin:
        try:
            print("Administracion")
            print("1. Ver todos los dulces")
            print("2. Agregar nuevo dulce")
            print("3. Modificar precio/stock de un dulce")
            print("4. Eliminar un dulce")
            print("5. Volver al Menu Principal")
            print("6. Ver Corte de Caja ")
            print("Que deseas hacer?: ", end="")
            print("")
            
            op = int(input())

            if op == 1:
                mostrar_inventario(inv)
            elif op == 2:
                cod = input("Codigo: ")
                nom = input("Nombre: ")
                pre = float(input("Precio: "))
                stk = int(input("Stock inicial: "))
                tip = input("Tipo: ")
                
                inv.agregar(Dulce(cod, nom, pre, stk, tip))
                print("Dulce agregado al catalogo")
            elif op == 3:
                mostrar_inventario(inv)
                cod = input("Escribe el codigo del dulce a modificar: ")
                d = inv.buscar_por_codigo(cod)
                
                nuevo_pre = float(input(f"Nuevo precio (Actual: {d.get_precio()}): $"))
                d.set_precio(nuevo_pre)
                
                add_stk = int(input(f"Agregar stock (Actual: {d.get_stock()}): "))
                d.set_stock(d.get_stock() + add_stk)
                
                inv.sincronizar_cambios()
                print("Dulce actualizado")
            elif op == 4:
                mostrar_inventario(inv)
                cod = input("Escribe el codigo del dulce a eliminar: ")
                d = inv.buscar_por_codigo(cod)
                inv.eliminar(d)
                print("Dulce eliminado")
            elif op == 5:
                en_admin = False
            elif op == 6:
                mostrar_corte_de_caja()
            else:
                print("Opcion fuera de rango.", file=sys.stderr)
        except ValueError:
            print("ERROR: Ingresa numeros, no letras.", file=sys.stderr)
        except InventarioException:
            pass

def menu_cliente(inv):
    global contador_ventas
    carrito_dulces = []
    carrito_cantidades = []
    comprando = True

    if not inv.obtener_todos():
        print("La tienda esta vacia ahorita", file=sys.stderr)
        return

    while comprando:
        try:
            print("")
            mostrar_inventario(inv)
            cod = input("Escribe el 'Codigo' del dulce para llevar (o '0' para ir a pagar):\n")

            if cod == "0":
                comprando = False
            else:
                d = inv.buscar_por_codigo(cod)
                cantidad = int(input(f"Cuantas piezas de {d.get_nombre()} quieres?: "))

                if cantidad <= 0:
                    print("Mete una cantidad valida.", file=sys.stderr)
                elif cantidad > d.get_stock():
                    raise InventarioException(f"Solo tenemos {d.get_stock()} piezas de {d.get_nombre()}")
                else:
                    d.set_stock(d.get_stock() - cantidad)
                    inv.sincronizar_cambios()
                    
                    carrito_dulces.append(d)
                    carrito_cantidades.append(cantidad)
                    print("Agregado al carrito")
        except ValueError:
            print("ERROR: La cantidad debe ser un numero entero.", file=sys.stderr)
        except InventarioException:
            pass

    if carrito_dulces:
        total = 0.0
        detalles_para_txt = ""

        print("----------------------------------")
        print("        TICKET DE COMPRA          ")
        print("----------------------------------")
        for d, cant in zip(carrito_dulces, carrito_cantidades):
            subtotal = d.get_precio() * cant
            total += subtotal
            print(f"{cant} x {d.get_nombre():<15} = ${subtotal:.2f}")
            detalles_para_txt += f"{cant}x {d.get_nombre()}, "
            
        print("----------------------------------")
        print(f"TOTAL A PAGAR:          ${total:.2f}")
        print("Gracias por tu compra en Dulceria Sosa!")
        print("")

        folio = f"V-{contador_ventas}"
        contador_ventas += 1
        nueva_venta = Venta(folio, total, detalles_para_txt)
        registrar_venta_txt(nueva_venta)

def main():
    inv = GestorInventario("inventario_dulceria.txt", desde_linea, a_linea)
    corriendo = True

    while corriendo:
        try:
            print("")
            print("Dulceria Sosa")
            print("1. Entrar en MODO ADMINISTRADOR")
            print("2. Entrar en MODO CLIENTE (Comprar)")
            print("3. Salir del sistema")
            print("Elige tu modo de acceso: ", end="")
            print("")
            
            modo = int(input())

            if modo == 1:
                menu_admin(inv)
            elif modo == 2:
                menu_cliente(inv)
            elif modo == 3:
                print("Apagando sistema...")
                corriendo = False
            else:
                print("Opcion no valida. Usa 1, 2 o 3.", file=sys.stderr)
        except ValueError:
            print("ERROR: Escribiste letras o simbolos. Solo usa numeros.", file=sys.stderr)

if __name__ == "__main__":
    main()