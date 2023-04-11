import UnityEngine
import netCDF4
import json
import os

input_paths = __name__.split('$')
output = []

# Get data from all files
for i in range(len(input_paths) - 1):
    path = input_paths[i]

    file_data = {
        "filePath": path,
        "position": {
            "lat": 0,
            "lon": 0
        },
        "size": {
            "x": 0,
            "y": 0
        }
    }

    try:
        with netCDF4.Dataset(path, "r") as dataset:
            file_data["position"]["lat"] = dataset.getncattr('origin_lat')
            file_data["position"]["lon"] = dataset.getncattr('origin_lon')

            x_values = [int(x) for x in dataset.variables['x'][:]]
            y_values = [int(y) for y in dataset.variables['y'][:]]

            file_data["size"]["x"] = x_values[-1] + x_values[0]
            file_data["size"]["y"] = y_values[-1] + y_values[0]

    except:
        print(f"Error reading {path}")
        UnityEngine.Debug.Log(f"Error reading {path}")

    output.append(file_data)

jsonPath = input_paths[len(input_paths) - 1]

if not os.path.exists(jsonPath):
    os.makedirs(jsonPath)

# Write data to output file
with open(jsonPath + "\\attributes.json", "w") as file:
    json.dump(output, file)
