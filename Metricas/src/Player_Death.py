import json
import os
import matplotlib.pyplot as plt

# Directorio donde están los JSON
telemetry_dir = "telemetria"

# Diccionario con salas y niveles
rooms_per_level = {
    "level_1": ["room_1", "room_2", "room_3"],
    "level_2": ["room_1", "room_2", "room_3", "room_4", "room_5"],
    "level_3": ["room_1", "room_2", "room_3"],
    "level_4": ["room_1", "room_2"]
}

# Inicializar contador con 0 para todas las salas
room_counter = {
    level: {room: 0 for room in rooms}
    for level, rooms in rooms_per_level.items()
}

# Función robusta para leer los eventos
def parse_concatenated_json(content):
    objects = []
    buffer = ""
    brace_count = 0

    for char in content:
        if char == "{":
            brace_count += 1
        if brace_count > 0:
            buffer += char
        if char == "}":
            brace_count -= 1
            if brace_count == 0:
                try:
                    objects.append(json.loads(buffer))
                except json.JSONDecodeError:
                    pass
                buffer = ""

    return objects

files_read = 0

# Leer dinámicamente TODOS los archivos .json dentro de la carpeta
if os.path.exists(telemetry_dir):
    for filename in os.listdir(telemetry_dir):
        if filename.endswith(".json"):
            filepath = os.path.join(telemetry_dir, filename)
            files_read += 1

            with open(filepath, "r", encoding="utf-8") as f:
                content = f.read()
                events = parse_concatenated_json(content)

                # Recorrer eventos buscando muertes
                for event in events:
                    if isinstance(event, dict) and event.get("eventType") == "Player_Death":
                        level_id = event.get("level_id")
                        room_id = event.get("room_id")

                        # Validar que el nivel y la sala existen en nuestro diccionario
                        if level_id in rooms_per_level and room_id in rooms_per_level[level_id]:
                            room_counter[level_id][room_id] += 1
else:
    print(f"Error: No se encontró la carpeta '{telemetry_dir}'")

# Preparar datos para el gráfico
labels = []
counts = []

for level, rooms in rooms_per_level.items():
    for room in rooms:
        # Formateamos las etiquetas visuales, ej: "L1 - R1"
        label = f"{level.replace('level_', 'L')} - {room.replace('room_', 'R')}"
        labels.append(label)
        counts.append(room_counter[level][room])

# Calcular porcentajes
total_deaths = sum(counts)
percentages = []

for count in counts:
    # Evitamos la división por cero
    if total_deaths > 0:
        pct = (count / total_deaths) * 100
    else:
        pct = 0.0
    percentages.append(pct)

# Debug útil
print(f"Archivos leídos: {files_read}")
print(f"Total Player_Death: {sum(counts)}")
print("Muertes por sala:")
for level, rooms in room_counter.items():
    print(f"  {level}: {rooms}")

# Crear carpeta si no existe
os.makedirs("graficos", exist_ok=True)

# Crear gráfico
plt.figure(figsize=(10, 6))
bars = plt.bar(labels, percentages, color='skyblue', edgecolor='black')

# Escribir el porcentaje encima de cada barra
for i, bar in enumerate(bars):
    yval = bar.get_height()
    # Colocamos el texto justo encima (yval + 1) y centrado
    plt.text(bar.get_x() + bar.get_width()/2, yval + 1, f'{round(yval, 1)}%({counts[i]})', 
                ha='center', va='bottom', fontweight='bold')
plt.xlabel("Nivel y Sala")
plt.ylabel("Porcentaje sobre el total (%)")
plt.title("Porcentaje de muertes por sala")
plt.xticks(rotation=45)

plt.tight_layout()
plt.savefig("graficos/grafico_muertes_por_sala.png")