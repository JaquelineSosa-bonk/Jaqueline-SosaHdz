# -*- coding: utf-8 -*-
import os
import sys
from inventario_exception import InventarioException

class GestorInventario:
    def __init__(self, ruta_archivo, conversor_desde_linea, conversor_a_linea):
        self.lista = []
        self.ruta_archivo = ruta_archivo
        self.conversor_desde_linea = conversor_desde_linea
        self.conversor_a_linea = conversor_a_linea
        self.cargar_datos()

    def agregar(self, elemento):
        self.lista.append(elemento)
        self.guardar_datos()

    def obtener_todos(self):
        return self.lista

    def buscar_por_codigo(self, codigo):
        for elemento in self.lista:
            if elemento.get_codigo() == codigo:
                return elemento
   
        raise InventarioException(f"El codigo '{codigo}' no existe en la base de datos.")

    def sincronizar_cambios(self):
        self.guardar_datos()

    def eliminar(self, elemento):
        self.lista.remove(elemento)
        self.guardar_datos()

    def cargar_datos(self):
        if not os.path.exists(self.ruta_archivo):
            return
        try:
            with open(self.ruta_archivo, 'r', encoding='utf-8') as f:
                for linea in f:
                    linea = linea.strip() 
                    if linea:
                        entidad = self.conversor_desde_linea(linea)
                        if entidad is not None:
                            self.lista.append(entidad)
        except Exception:
            print("Error al cargar los datos.", file=sys.stderr)

    def guardar_datos(self):
        try:
            with open(self.ruta_archivo, 'w', encoding='utf-8') as f:
                for e in self.lista:
                    f.write(self.conversor_a_linea(e) + '\n')
        except Exception:
            print("Error al guardar.", file=sys.stderr)