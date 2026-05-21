package Clases;

import java.io.*;
import java.util.ArrayList;
import java.util.function.Function;

public class GestorInventario<T extends Producto> {
    private ArrayList<T> lista;
    private final String rutaArchivo;
    
    private final Function<String, T> conversorDesdeLinea;
    private final Function<T, String> conversorALinea;

    public GestorInventario(String rutaArchivo, Function<String, T> desdeLinea, Function<T, String> aLinea) {
        this.lista = new ArrayList<>();
        this.rutaArchivo = rutaArchivo;
        this.conversorDesdeLinea = desdeLinea;
        this.conversorALinea = aLinea;
        cargarDatos();
    }

    public void agregar(T elemento) {
        lista.add(elemento);
        guardarDatos();
    }

    public ArrayList<T> obtenerTodos() {
        return lista;
    }

    public T buscarPorCodigo(String codigo) throws InventarioException {
        for (T elemento : lista) {
            if (elemento.getCodigo().equals(codigo)) {
                return elemento;
            }
        }
        throw new InventarioException("El código '" + codigo + "' no existe en la base de datos.");
    }

    public void sincronizarCambios() {
        guardarDatos();
    }

    public void eliminar(T elemento) {
        lista.remove(elemento);
        guardarDatos();
    }

    private void cargarDatos() {
        File archivo = new File(rutaArchivo);
        if (!archivo.exists()) return;
        try (BufferedReader br = new BufferedReader(new FileReader(archivo))) {
            String linea;
            while ((linea = br.readLine()) != null) {
                T entidad = conversorDesdeLinea.apply(linea);
                if (entidad != null) lista.add(entidad);
            }
        } catch (Exception e) {
            System.err.println("Error al cargar los datos.");
        }
    }

    private void guardarDatos() {
        try (PrintWriter pw = new PrintWriter(new FileWriter(rutaArchivo))) {
            for (T e : lista) {
                pw.println(conversorALinea.apply(e));
            }
        } catch (Exception e) {
            System.err.println("Error al guardar.");
        }
    }
}