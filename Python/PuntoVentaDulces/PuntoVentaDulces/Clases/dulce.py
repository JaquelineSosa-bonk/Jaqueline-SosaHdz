
from producto import Producto

class Dulce(Producto):
    def __init__(self, codigo, nombre, precio, stock, tipo):

        super().__init__(codigo, nombre, precio, stock) 
        self.tipo = tipo

    def get_tipo(self): 
        return self.tipo

    def __str__(self):
        return super().__str__() + f" | Tipo: {self.tipo}"