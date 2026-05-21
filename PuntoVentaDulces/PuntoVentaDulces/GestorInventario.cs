using System;
using System.Collections.Generic;
using System.IO;

namespace Clases
{
    public class GestorInventario<T> where T : Producto
    {
        private List<T> lista;
        private readonly string rutaArchivo;

        private readonly Func<string, T> conversorDesdeLinea;
        private readonly Func<T, string> conversorALinea;

        public GestorInventario(string rutaArchivo, Func<string, T> desdeLinea, Func<T, string> aLinea)
        {
            this.lista = new List<T>();
            this.rutaArchivo = rutaArchivo;
            this.conversorDesdeLinea = desdeLinea;
            this.conversorALinea = aLinea;
            CargarDatos();
        }

        public void Agregar(T elemento)
        {
            lista.Add(elemento);
            GuardarDatos();
        }

        public List<T> ObtenerTodos()
        {
            return lista;
        }

        public T BuscarPorCodigo(string codigo)
        {
            foreach (T elemento in lista)
            {
                if (elemento.GetCodigo() == codigo)
                {
                    return elemento;
                }
            }
            throw new InventarioException("El codigo '" + codigo + "' no existe en la base de datos.");
        }

        public void SincronizarCambios()
        {
            GuardarDatos();
        }

        public void Eliminar(T elemento)
        {
            lista.Remove(elemento);
            GuardarDatos();
        }

        private void CargarDatos()
        {
            if (!File.Exists(rutaArchivo)) return;

            try
            {
                using (StreamReader br = new StreamReader(rutaArchivo))
                {
                    string linea;
                    while ((linea = br.ReadLine()) != null)
                    {
         
                        T entidad = conversorDesdeLinea(linea);
                        if (entidad != null) lista.Add(entidad);
                    }
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error al cargar los datos.");
            }
        }

        private void GuardarDatos()
        {
            try
            {
                using (StreamWriter pw = new StreamWriter(rutaArchivo))
                {
                    foreach (T e in lista)
                    {
                        pw.WriteLine(conversorALinea(e));
                    }
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error al guardar.");
            }
        }
    }
}