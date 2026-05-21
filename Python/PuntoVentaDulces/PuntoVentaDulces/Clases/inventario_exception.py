import time
import sys

class InventarioException(Exception):
    def __init__(self, mensaje):
        super().__init__(mensaje)
        
        print(f"Alerta del sistema{mensaje}", file=sys.stderr)

        time.sleep(0.2)