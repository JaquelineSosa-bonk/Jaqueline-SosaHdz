class Producto:
    def __init__(self, codigo, nombre, precio, stock):
        self.codigo = codigo
        self.nombre = nombre
        self.precio = float(precio)
        self.stock = int(stock)


    def get_codigo(self): 
        return self.codigo
        
    def get_nombre(self): 
        return self.nombre
        
    def get_precio(self): 
        return self.precio
        
    def get_stock(self): 
        return self.stock
    
    def set_precio(self, precio): 
        self.precio = float(precio)
        
    def set_stock(self, stock): 
        self.stock = int(stock)

    def __str__(self):
       
        return f"[{self.codigo}] {self.nombre:<15} | Precio: ${self.precio:.2f} | Stock: {self.stock}"