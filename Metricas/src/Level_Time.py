import json
import os
import matplotlib.pyplot as plt
from collections import defaultdict

# Configuración
base_filename = "telemetria/telemetry_{}.json"
max_files = 100

# Levels posibles (fijos)
valid_levels = ["level_1", "level_2", "level_3", "level_4"]

# Guardamos tiempos
start_times = defaultdict(list)
total_time = defaultdict(float)

files_read = 0


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
for i in range(max_files):
    filename = base_filename.format(i)

    if not os.path.exists(filename):
        continue

    files_read += 1

    with open(filename, "r", encoding="utf-8") as f:
        content = f.read()
        events = parse_concatenated_json(content)

        for event in events:
            if not isinstance(event, dict):
                continue

            event_type = event.get("eventType")
            level_id = event.get("level_id")
            timestamp = event.get("timeStamp")

            if level_id not in valid_levels or timestamp is None:
                continue

            # Guardar inicio
            if event_type == "Level_Start":
                start_times[level_id].append(timestamp)

            # Calcular duración al completar
            elif event_type == "Level_Complete":
                if start_times[level_id]:
                    start_time = start_times[level_id].pop(0)
                    duration = timestamp - start_time
                    if duration > 0:
                        total_time[level_id] += duration


# Preparar datos para gráfico
levels = valid_levels
times = [total_time[level] for level in levels]

print(f"Archivos leídos: {files_read}")
print("Tiempo total por nivel:", dict(total_time))

# Crear carpeta si no existe
os.makedirs("graficos", exist_ok=True)

# Gráfico
plt.figure()
plt.bar(levels, times)
plt.xlabel("Level ID")
plt.ylabel("Tiempo total (s)")
plt.title("Tiempo total por nivel (Level_Start → Level_Complete)")
plt.xticks(rotation=45)

plt.tight_layout()
plt.savefig("graficos/grafico_tiempo_niveles.png")