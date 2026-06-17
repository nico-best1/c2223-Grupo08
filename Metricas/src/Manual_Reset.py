import json
import os
import matplotlib.pyplot as plt
from collections import Counter

os.makedirs("graficos", exist_ok=True)

# Configuración
base_filename = "telemetria/telemetry_{}.json"
max_files = 100  # Ajusta según necesidad

# Levels posibles (fijos)
valid_levels = ["level_1", "level_2", "level_3", "level_4"]

# Inicializar contador con 0 para todos
level_counter = Counter({level: 0 for level in valid_levels})

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

                if level_id in valid_levels:
                    level_counter[level_id] += 1

# Preparar datos SIEMPRE (aunque sean 0)
levels = valid_levels
counts = [level_counter[level] for level in levels]

# Debug útil
print(f"Archivos leídos: {files_read}")
print(f"Total Manual_Reset: {sum(counts)}")

# Crear gráfico (aunque todo sea 0)
plt.figure()
plt.bar(levels, counts)
plt.xlabel("Level ID")
plt.ylabel("Número de veces (Manual_Reset)")
plt.title("Numero de resets por nivel")
plt.xticks(rotation=45)

plt.tight_layout()
plt.savefig("graficos/grafico_resets.png")