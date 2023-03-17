import UnityEngine
import netCDF4
import csv
import os

input_fields = __name__.split('$')

nc_path = input_fields[0]
variable_name = input_fields[1]
output_path = input_fields[2]

# Check the shape of the variable data and create either a single CSV file or multiple CSV files
with netCDF4.Dataset(nc_path, "r") as nc_file:
    variable_data = nc_file.variables[variable_name][:]

    if len(variable_data.shape) == 2:

        # Create a single CSV file for the variable data
        output_path = output_path + ".csv"
        dir = os.path.dirname(output_path)

        if not os.path.exists(dir):
            os.makedirs(dir)

        with open(output_path, "w", newline="") as csv_file:
            writer = csv.writer(csv_file)
            for row in variable_data:
                writer.writerow(row)


    elif len(variable_data.shape) == 4:
        output_path += "\\"

        if not os.path.exists(output_path):
            os.makedirs(output_path)

        # Create multiple CSV files for the variable data
        for i in range(variable_data.shape[0]):

            time_value = int(nc_file.variables['time'][i])
            new_output_path = output_path + "\\" + str(time_value) + ".csv"
            data = variable_data[i, 0]

            with open(new_output_path, "w", newline="") as csv_file:
                writer = csv.writer(csv_file)
                for row in data:
                    writer.writerow(row)

    else:
        print("Variable data has an unsupported shape")
        UnityEngine.Debug.Log("Variable data has an unsupported shape: " + variable_name)