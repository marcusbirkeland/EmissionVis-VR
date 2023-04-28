import UnityEngine;
import netCDF4
import json
import os

# Split the input string by '$' to get the individual file paths and an output path
input_paths = __name__.split('$')
output = []

# Iterate through each input file path
for i in range(len(input_paths) - 1):
    path = input_paths[i]

    # Initialize file_data dictionary with file path and an empty list of variables
    file_data = {
        "filePath": path,
        "variables": []
    }

    # Read the variables from the NetCDF dataset
    try:
        with netCDF4.Dataset(path, "r") as dataset:
            for var in dataset.variables:
                file_data["variables"].append(var)
    except:
        print(f"Error reading {path}")
        UnityEngine.Debug.Log(f"Error reading {path}")

    # Append the file_data dictionary to the output list
    output.append(file_data)

# Get the JSON output file path
jsonPath = input_paths[len(input_paths) - 1]

# Create the output directory if it does not exist
if not os.path.exists(jsonPath):
    os.makedirs(jsonPath)

#Write the output list to a JSON file
with open(jsonPath + "\\variables.json", "w") as file:
    json.dump(output, file)
