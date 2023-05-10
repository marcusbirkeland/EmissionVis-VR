import UnityEngine
import matplotlib
matplotlib.use('Agg')

import netCDF4
import os
import numpy as np
from matplotlib import pyplot as plt
from scipy.interpolate import interp2d

# Split the input string by '$' to get the individual input values
input_fields = __name__.split('$')

nc_path = input_fields[0]
variable_name = input_fields[1]
output_path = input_fields[2]
interpolation_factor = int(input_fields[3])

try:
    # Open the NetCDF file and get the variable data
    with netCDF4.Dataset(nc_path, "r") as nc_file:
        variable_data = nc_file.variables[variable_name][:]

        # Check the shape of the variable data
        if len(variable_data.shape) == 2:

            # Create a single PNG file for the variable data
            output_path = output_path + ".png"

            # Get the variable
            nc_var = nc_file.variables[variable_name]

            # Read the data from the NetCDF file
            data = nc_var[:, :]

            # Create a 2D interpolation of the data
            x = np.arange(data.shape[1])
            y = np.arange(data.shape[0])
            f = interp2d(x, y, data, kind='linear')

            # Generate a grid of x and y values for the interpolated data
            x_new = np.linspace(x.min(), x.max(), data.shape[1] * interpolation_factor)
            y_new = np.linspace(y.min(), y.max(), data.shape[0] * interpolation_factor)

            # Interpolate the data to the new grid
            interpolated_data = f(x_new, y_new)

            # Flip the interpolated data vertically
            flipped_data = np.flipud(interpolated_data)

            # Add a 1-pixel black border around the image
            bordered_data = np.pad(flipped_data, pad_width=1, mode='constant', constant_values=0)

            # Create output directory if it does not exist
            dir_path = os.path.dirname(output_path)
            if not os.path.exists(dir_path):
                os.makedirs(dir_path)

            # Save the plot as a PNG file
            plt.imsave(output_path, bordered_data, cmap='gray')

        else:
            print("Variable data has an unsupported shape")
            UnityEngine.Debug.Log("Variable data has an unsupported shape: " + variable_name)

except Exception as e:
    error_msg = f"Error processing {nc_path}: {str(e)}"
    print(error_msg)
    UnityEngine.Debug.LogError(error_msg)
