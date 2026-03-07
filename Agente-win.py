import socket
import struct
import json
import subprocess
import platform
import time
from datetime import datetime

# CONFIGURACIÓN: Cambia esto por la IP de tu servidor donde corre el .NET
HOST, PORT = "192.168.1.18", 5001 

def send_json(sock, obj):
    data = json.dumps(obj).encode("utf-8")
    sock.sendall(struct.pack("!I", len(data)) + data)

def recv_json(sock):
    header = sock.recv(4)
    if not header: return None
    length = struct.unpack("!I", header)[0]
    return json.loads(sock.recv(length).decode("utf-8"))

def main():
    hostname = platform.node()
    print(f"[*] Agente iniciado en: {hostname}")
    
    while True:
        try:
            print(f"[*] Intentando conectar a {HOST}:{PORT}...")
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
                s.connect((HOST, PORT))
                
                # ENVIAR SALUDO INICIAL
                print(f"[+] ¡Conectado exitosamente al servidor!")
                send_json(s, {
                    "type": "handshake",
                    "hostname": hostname,
                    "status": "online",
                    "ts": datetime.now().strftime("%H:%M:%S")
                })
                
                while True:
                    msg = recv_json(s)
                    if not msg: break
                    
                    if msg.get("type") == "exec":
                        print(f"[>] Ejecutando comando: {msg.get('comando')}")
                        # ... (resto de tu lógica de ejecución de subprocess) ...
                        send_json(s, {"type": "result", "matched": True, "stderr": ""})

        except ConnectionRefusedError:
            print("[-] Servidor no disponible. Reintentando en 5s...")
            time.sleep(5)
        except Exception as e:
            print(f"[!] Error: {e}")
            time.sleep(5)

if __name__ == "__main__":
    main()