import netCDF4
import csv
import os

input_paths = __name__.split('$')

datapath = input_paths[0]
varible_name = input_paths[1]
csv_path = input_paths[2]

nc = netCDF4.Dataset(datapath, "r")
buildings = nc.variables[varible_name][:]

# Read the global attributes for X and Y values
x_values = []
y_values = []

for x in nc.variables['x'][:]:
    x_values.append(int(x))

for y in nc.variables['y'][:]:
    y_values.append(int(y))


# Write the non-zero X, Y values to a CSV file
csv_file = csv_path + ".csv"

if not os.path.exists(os.path.dirname(csv_path)):
    os.makedirs(os.path.dirname(csv_path))

with open(csv_file, "w", newline='') as csvfile:
    csv_writer = csv.writer(csvfile)

    for i in range(buildings.shape[0]):
        for j in range(buildings.shape[1]):
            if buildings[i, j] != 0:
                x_value = x_values[i]
                y_value = y_values[j]
                csv_writer.writerow([x_value, y_value])
