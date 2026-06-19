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

# Inicializar contador con 0 para todas las salas de todos los niveles
room_counter = {
    level: {room: 0 for room in rooms}
    for level, rooms in rooms_per_level.items()
}

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

# Leer archivos
files_read = 0

if os.path.exists(telemetry_dir):
    for filename in os.listdir(telemetry_dir):
        if filename.endswith(".json"):
            filepath = os.path.join(telemetry_dir, filename)
            files_read += 1

            with open(filepath, "r", encoding="utf-8") as f:
                content = f.read()
                events = parse_concatenated_json(content)

                # Recorrer eventos buscando el reinicio manual
                for event in events:
                    if isinstance(event, dict) and event.get("eventType") == "Manual_Reset":
                        level_id = event.get("level_id")
                        room_id = event.get("room_id")

                        # Validar que el nivel y la sala existen en nuestro diccionario
                        if level_id in rooms_per_level and room_id in rooms_per_level[level_id]:
                            room_counter[level_id][room_id] += 1
else:
    print(f"Error: No se encontró la carpeta '{telemetry_dir}'")

# Preparar datos
labels = []
counts = []

for level, rooms in rooms_per_level.items():
    for room in rooms:
        # Formateamos las etiquetas para que sean legibles, ej: "L1 - R1"
        label = f"{level.replace('level_', 'L')} - {room.replace('room_', 'R')}"
        labels.append(label)
        counts.append(room_counter[level][room])

# Calcular porcentajes
total_resets = sum(counts)
percentages = []

for count in counts:
    # Evitamos la división por cero si no hay ningún reset registrado
    if total_resets > 0:
        pct = (count / total_resets) * 100
    else:
        pct = 0.0
    percentages.append(pct)

# Debug útil
print(f"Archivos leídos: {files_read}")
print(f"Total Manual_Reset: {sum(counts)}")

os.makedirs("graficos", exist_ok=True)

# Crear gráfico 
plt.figure(figsize=(10, 6))
plt.bar(labels, percentages, color='skyblue', edgecolor='black')
plt.xlabel("Nivel y Sala")
plt.ylabel("Porcentaje sobre el total (%)")
plt.title("Porcentaje de resets por sala")
plt.xticks(rotation=45)

plt.tight_layout()
plt.savefig("graficos/grafico_resets_por_sala.png")