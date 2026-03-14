import sys
import os

# Forzar backend sin pantalla ANTES de importar pyplot
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches

def main():
    print(f"[grafica.py] Argumentos recibidos: {sys.argv}", flush=True)

    if len(sys.argv) < 6:
        print("ERROR: Se necesitan 5 argumentos: ruta_salida Completado EnProgreso Pendiente Fallido", flush=True)
        sys.exit(1)

    ruta_salida  = sys.argv[1]
    completado   = int(sys.argv[2])
    en_progreso  = int(sys.argv[3])
    pendiente    = int(sys.argv[4])
    fallido      = int(sys.argv[5])

    print(f"[grafica.py] Ruta salida: {ruta_salida}", flush=True)
    print(f"[grafica.py] Datos: Completado={completado} EnProgreso={en_progreso} Pendiente={pendiente} Fallido={fallido}", flush=True)

    # Crear directorio si no existe
    directorio = os.path.dirname(os.path.abspath(ruta_salida))
    print(f"[grafica.py] Creando directorio: {directorio}", flush=True)
    os.makedirs(directorio, exist_ok=True)

    labels  = ['Completado', 'En Progreso', 'Pendiente', 'Fallido']
    valores = [completado, en_progreso, pendiente, fallido]
    colores = ['#28a745', '#007bff', '#ffc107', '#dc3545']

    # Filtrar categorías con valor 0
    datos = [(l, v, c) for l, v, c in zip(labels, valores, colores) if v > 0]

    fig, ax = plt.subplots(figsize=(6, 5))

    if not datos:
        ax.text(0.5, 0.5, 'Sin datos de escaneos',
                ha='center', va='center', fontsize=14, color='gray',
                transform=ax.transAxes)
        ax.axis('off')
    else:
        lf = [d[0] for d in datos]
        vf = [d[1] for d in datos]
        cf = [d[2] for d in datos]

        wedges, texts, autotexts = ax.pie(
            vf,
            labels=None,
            colors=cf,
            autopct=lambda p: f'{p:.1f}%\n({int(round(p * sum(vf) / 100))})',
            startangle=140,
            pctdistance=0.75,
            wedgeprops=dict(width=0.55, edgecolor='white', linewidth=2)
        )

        for at in autotexts:
            at.set_fontsize(9)
            at.set_color('white')
            at.set_fontweight('bold')

        leyenda = [mpatches.Patch(color=c, label=f'{l}: {v}')
                   for l, v, c in zip(lf, vf, cf)]
        ax.legend(handles=leyenda, loc='lower center',
                  bbox_to_anchor=(0.5, -0.15), ncol=2,
                  fontsize=9, frameon=False)

    ax.set_title('Estados de Escaneos', fontsize=13,
                 fontweight='bold', pad=15, color='#333333')

    plt.tight_layout()

    ruta_absoluta = os.path.abspath(ruta_salida)
    print(f"[grafica.py] Guardando imagen en: {ruta_absoluta}", flush=True)

    plt.savefig(ruta_absoluta, dpi=120, bbox_inches='tight',
                facecolor='white', transparent=False)
    plt.close()

    if os.path.exists(ruta_absoluta):
        print(f"[grafica.py] OK - Imagen creada ({os.path.getsize(ruta_absoluta)} bytes)", flush=True)
    else:
        print("[grafica.py] ERROR - La imagen no se creó", flush=True)
        sys.exit(1)

if __name__ == '__main__':
    main()