import json
import os
import matplotlib.pyplot as plt
from collections import defaultdict

# Directorio donde están los JSON
telemetry_dir = "telemetria"

# Diccionario con salas y niveles
rooms_per_level = {
    "level_1": ["room_1", "room_2", "room_3"],
    "level_2": ["room_1", "room_2", "room_3", "room_4", "room_5"],
    "level_3": ["room_1", "room_2", "room_3"],
    "level_4": ["room_1", "room_2"]
}

# Guardamos tiempos y sesiones únicas
start_times = {
    level: {room: [] for room in rooms}
    for level, rooms in rooms_per_level.items()
}
total_time = {
    level: {room: 0 for room in rooms}
    for level, rooms in rooms_per_level.items()
}
unique_sessions = {
    level: {room: set() for room in rooms}
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

files_read = 0

# Leer archivos
if os.path.exists(telemetry_dir):
    for filename in os.listdir(telemetry_dir):
        if filename.endswith(".json"):
            filepath = os.path.join(telemetry_dir, filename)
            files_read += 1

            with open(filepath, "r", encoding="utf-8") as f:
                content = f.read()
                events = parse_concatenated_json(content)

                for event in events:
                    if not isinstance(event, dict):
                        continue

                    event_type = event.get("eventType")
                    level_id = event.get("level_id")
                    room_id = event.get("room_id")
                    timestamp = event.get("timeStamp")
                    session_id = event.get("sessionId")

                    if level_id not in rooms_per_level or room_id not in rooms_per_level[level_id] or timestamp is None:
                        continue

                    # Guardar inicio
                    if event_type == "Room_Start":
                        start_times[level_id][room_id].append(timestamp)

                    # Calcular duración al completar la sala o al morir
                    elif event_type in ["Room_Complete", "Player_Death"]:
                        if start_times[level_id][room_id]:
                            start_time = start_times[level_id][room_id].pop(-1)
                            start_times[level_id][room_id].clear()
                            
                            duration = (timestamp - start_time) / 1000
                            if duration > 0:
                                total_time[level_id][room_id] += duration
                                # Registramos la sesión
                                if session_id:
                                    unique_sessions[level_id][room_id].add(session_id)
else:
    print(f"Error: No se encontró la carpeta '{telemetry_dir}'")


# Preparar datos para gráfico
labels = []
counts = []
averages_for_print = {}

for level, rooms in rooms_per_level.items():
    averages_for_print[level] = {}
    for room in rooms:
        # Formateamos las etiquetas para que sean legibles, ej: "L1 - R1"
        label = f"{level.replace('level_', 'L')} - {room.replace('room_', 'R')}"
        labels.append(label)

        # Cálculo de la media
        total = total_time[level][room]
        session_count = len(unique_sessions[level][room])

        # Evitamos la división por cero si nadie ha jugado la sala
        if session_count > 0:
            avg_time = total / session_count
        else:
            avg_time = 0.0
            
        counts.append(avg_time)
        averages_for_print[level][room] = round(avg_time, 2)


print(f"Archivos leídos: {files_read}")
print("Tiempo total por nivel:", dict(total_time))

# Crear carpeta si no existe
os.makedirs("graficos", exist_ok=True)

# Gráfico
plt.figure(figsize=(10, 6))
plt.bar(labels, counts, color='skyblue', edgecolor='black')
plt.xlabel("Nivel y Sala")
plt.ylabel("Tiempo medio por sesión (s)")
plt.title("Tiempo medio por sala")
plt.xticks(rotation=45)

plt.tight_layout()
plt.savefig("graficos/grafico_tiempo_por_sala.png")