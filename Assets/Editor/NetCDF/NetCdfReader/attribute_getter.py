import UnityEngine
import netCDF4
import json
import os

# Split the input string by '$' to get the individual file paths
input_paths = __name__.split('$')
output = []

# Process each NetCDF file in the input_paths list
for i in range(len(input_paths) - 1):
    path = input_paths[i]

    # Initialize a dictionary to store the data extracted from the current NetCDF file
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

    # Read data from the NetCDF file
    try:
        with netCDF4.Dataset(path, "r") as dataset:
            # Get origin latitude and longitude from the file and store them in the 'position' dictionary
            file_data["position"]["lat"] = dataset.getncattr('origin_lat')
            file_data["position"]["lon"] = dataset.getncattr('origin_lon')

            # Get x and y values from the file
            x_values = [int(x) for x in dataset.variables['x'][:]]
            y_values = [int(y) for y in dataset.variables['y'][:]]

            # Calculate the size in x and y directions and store them in the 'size' dictionary
            file_data["size"]["x"] = x_values[-1] + x_values[0]
            file_data["size"]["y"] = y_values[-1] + y_values[0]

    except:
        print(f"Error reading {path}")
        UnityEngine.Debug.Log(f"Error reading {path}")

    # Add the extracted data to the output list
    output.append(file_data)

# Get the output folder path from the input_paths list
jsonPath = input_paths[len(input_paths) - 1]

# Create the output folder if it doesn't exist
if not os.path.exists(jsonPath):
    os.makedirs(jsonPath)

# Write the output data to a JSON file in the output folder
with open(jsonPath + "\\attributes.json", "w") as file:
    json.dump(output, file)
