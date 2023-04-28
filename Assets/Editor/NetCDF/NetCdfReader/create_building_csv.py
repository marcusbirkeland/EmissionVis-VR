import netCDF4
import csv
import os

#NOTE: This script assumes that the buildings and height data are the same size

# Split the input string by '$' to get the individual file paths and variable names
input_paths = __name__.split('$')

buildings_datapath = input_paths[0]
buildings_variable_name = input_paths[1]
height_datapath = input_paths[2]
height_variable_name = input_paths[3]
output_path = input_paths[4]

# Read the buildings and height data from the NetCDF files
nc = netCDF4.Dataset(buildings_datapath, "r")
buildings = nc.variables[buildings_variable_name][:]

ncHeight = netCDF4.Dataset(height_datapath, "r")
height = ncHeight.variables[height_variable_name][:]

# Read the global attributes for X and Y values from the buildings data file
x_values = []
y_values = []

for x in nc.variables['x'][:]:
    x_values.append(int(x))

for y in nc.variables['y'][:]:
    y_values.append(int(y))


# Write the non-zero X, Y, and height values to a CSV file
csv_file = output_path + ".csv"

# Create the output folder if it doesn't exist
if not os.path.exists(os.path.dirname(output_path)):
    os.makedirs(os.path.dirname(output_path))

# Write the data to the CSV file
with open(csv_file, "w", newline='') as csvfile:
    csv_writer = csv.writer(csvfile)

    # Iterate through the buildings data
    for i in range(buildings.shape[0]):
        for j in range(buildings.shape[1]):
            
            # Only write rows with non-zero building values
            if buildings[i, j] != 0:
                x_value = x_values[i]
                y_value = y_values[j]
                z_value = int(height[i, j])
                csv_writer.writerow([x_value, y_value, z_value])
