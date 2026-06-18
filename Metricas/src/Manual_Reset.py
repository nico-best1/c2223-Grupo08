import json
import os
import matplotlib.pyplot as plt

os.makedirs("graficos", exist_ok=True)

# Configuración
base_filename = "telemetria/telemetry_{}.json"
max_files = 100  # Ajusta según necesidad

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

# Leer archivos
files_read = 0

for i in range(max_files):
    filename = base_filename.format(i)

    if not os.path.exists(filename):
        continue

    files_read += 1

    with open(filename, "r", encoding="utf-8") as f:
        try:
            data = json.load(f)

            # Caso 1: lista de eventos
            if isinstance(data, list):
                events = data
            # Caso 2: objeto con clave tipo "events"
            elif isinstance(data, dict):
                events = data.get("events", [])
            else:
                events = []

        except json.JSONDecodeError:
            # Caso 3: JSON por líneas (NDJSON)
            f.seek(0)
            events = []
            for line in f:
                line = line.strip()
                if not line:
                    continue
                try:
                    events.append(json.loads(line))
                except json.JSONDecodeError:
                    print(f"Línea inválida en {filename}")

        # Asumimos lista de eventos
        for event in events:
            if (
                isinstance(event, dict) and
                event.get("eventType") == "Manual_Reset"
            ):
                level_id = event.get("level_id")
                room_id = event.get("room_id")

                # Validar que el nivel y la sala existen en nuestro diccionario
                if level_id in rooms_per_level and room_id in rooms_per_level[level_id]:
                    room_counter[level_id][room_id] += 1

# Preparar datos
labels = []
counts = []

for level, rooms in rooms_per_level.items():
    for room in rooms:
        # Formateamos las etiquetas para que sean legibles, ej: "L1 - R1"
        label = f"{level.replace('level_', 'L')} - {room.replace('room_', 'R')}"
        labels.append(label)
        counts.append(room_counter[level][room])

# Debug útil
print(f"Archivos leídos: {files_read}")
print(f"Total Manual_Reset: {sum(counts)}")

# Crear gráfico (aunque todo sea 0)
plt.figure(figsize=(10, 6))
plt.bar(labels, counts, color='skyblue', edgecolor='black')
plt.xlabel("Nivel y Sala")
plt.ylabel("Número de veces (Manual_Reset)")
plt.title("Numero de resets por sala")
plt.xticks(rotation=45)

plt.tight_layout()
plt.savefig("graficos/grafico_resets_por_sala.png")