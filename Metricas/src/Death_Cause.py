import json
import os
import matplotlib.pyplot as plt
from collections import defaultdict

telemetry_dir = "telemetria"

rooms_per_level = {
    "level_1": ["room_1", "room_2", "room_3"],
    "level_2": ["room_1", "room_2", "room_3", "room_4", "room_5"],
    "level_3": ["room_1", "room_2", "room_3"],
    "level_4": ["room_1", "room_2"]
}

data = {
    level: {room: defaultdict(int) for room in rooms}
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

if os.path.exists(telemetry_dir):
    for filename in os.listdir(telemetry_dir):
        if filename.endswith(".json"):
            filepath = os.path.join(telemetry_dir, filename)
            files_read += 1

            with open(filepath, "r", encoding="utf-8") as f:
                content = f.read()
                events = parse_concatenated_json(content)

                for event in events:
                    if isinstance(event, dict) and event.get("eventType") == "Player_Death":
                        level_id = event.get("level_id")
                        room_id = event.get("room_id")
                        cause = event.get("cause", "unknown")

                        if level_id in data and room_id in data[level_id]:
                            data[level_id][room_id][cause] += 1
else:
    print(f"Error: No se encontró la carpeta '{telemetry_dir}'")
    exit(1)

all_causes = set()
for level in data:
    for room in data[level]:
        all_causes.update(data[level][room].keys())
all_causes = sorted(all_causes)

labels = []
for level, rooms in rooms_per_level.items():
    for room in rooms:
        label = f"{level.replace('level_', 'L')} - {room.replace('room_', 'R')}"
        labels.append(label)

totals = []
room_cause_counts = []
for level, rooms in rooms_per_level.items():
    for room in rooms:
        row = [data[level][room][c] for c in all_causes]
        room_cause_counts.append(row)
        totals.append(sum(row))

total_all = sum(totals)
print(f"Archivos leidos: {files_read}")
print(f"Total Player_Death: {total_all}")
print("\nMuertes por sala y causa:")
for i, (level, rooms) in enumerate(rooms_per_level.items()):
    for room in rooms:
        idx = labels.index(f"{level.replace('level_', 'L')} - {room.replace('room_', 'R')}")
        t = totals[idx]
        if t > 0:
            print(f"  {labels[idx]} ({t})")
            for j, cause in enumerate(all_causes):
                c = room_cause_counts[idx][j]
                if c > 0:
                    print(f"    {cause}: {c} ({c/t*100:.1f}%)")

os.makedirs("graficos", exist_ok=True)

plt.figure(figsize=(12, 6))
x = range(len(labels))
bottom = [0] * len(labels)

colors = plt.cm.Set2.colors[:len(all_causes)]

for j, cause in enumerate(all_causes):
    values = [room_cause_counts[i][j] for i in range(len(labels))]
    bars = plt.bar(x, values, bottom=bottom, label=cause, color=colors[j % len(colors)], edgecolor='black', linewidth=0.5)
    for i, bar in enumerate(bars):
        h = bar.get_height()
        if h > 0:
            pct = h / totals[i] * 100 if totals[i] > 0 else 0
            plt.text(bar.get_x() + bar.get_width()/2, bar.get_y() + h/2,
                     f'{pct:.0f}%', ha='center', va='center', fontsize=7, fontweight='bold', color='white')
    bottom = [bottom[i] + values[i] for i in range(len(labels))]

plt.xlabel("Nivel y Sala")
plt.ylabel("Numero de muertes")
plt.title("Distribucion de causas de muerte por sala")
plt.xticks(list(x), labels, rotation=45)
plt.legend(title="Causa")

plt.tight_layout()
plt.savefig("graficos/grafico_muertes_por_causa.png")

print(f"\nGrafico guardado en: graficos/grafico_muertes_por_causa.png")
