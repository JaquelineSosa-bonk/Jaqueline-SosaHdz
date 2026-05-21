from datetime import datetime

class Venta:
    def __init__(self, folio, total, detalles):
        self.folio = folio
        self.total = float(total)
        self.detalles = detalles
        
        ahora = datetime.now()
        self.fecha = ahora.strftime("%d/%m/%Y %H:%M:%S")

    def get_folio(self): 
        return self.folio
        
    def get_fecha(self): 
        return self.fecha
        
    def get_total(self): 
        return self.total
        
    def get_detalles(self): 
        return self.detalles

    def a_linea_txt(self):
        return f"{self.folio}|{self.fecha}|{self.total}|{self.detalles}"