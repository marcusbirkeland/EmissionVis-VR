import UnityEngine;
import netCDF4
import json
import os

input_paths = __name__.split('$')
output = []

#Get data from all files
for i in range(len(input_paths) - 1):

    path = input_paths[i]

    file_data = {
        "filePath": path,
        "variables": []
    }

    try:
        with netCDF4.Dataset(path, "r") as dataset:
            for var in dataset.variables:
                file_data["variables"].append(var)
    except:
        print(f"Error reading {path}")
        UnityEngine.Debug.Log(f"Error reading {path}")

    output.append(file_data)

jsonPath = input_paths[len(input_paths) - 1]

if not os.path.exists(jsonPath):
    os.makedirs(jsonPath)

#Write data to outputfile
with open(jsonPath + "\\variables.json", "w") as file:
    json.dump(output, file)

