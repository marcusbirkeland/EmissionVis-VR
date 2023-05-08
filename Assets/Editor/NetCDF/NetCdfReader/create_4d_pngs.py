import UnityEngine
import matplotlib
matplotlib.use('Agg')

import netCDF4
import os
import numpy as np
import cv2 as cv
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

        # If the variable data is 4-dimensional, create multiple PNG files
        if len(variable_data.shape) == 4:

            output_path += "\\"

            # Iterate through the time dimension
            for i in range(variable_data.shape[0]):

                time_value = int(nc_file.variables['time'][i])
                new_output_path = output_path + "{}.png".format(time_value)
                data = variable_data[i, 0]

                # Create a 2D interpolation of the data
                x = np.arange(data.shape[1])
                y = np.arange(data.shape[0])
                f = interp2d(x, y, data, kind='cubic')

                # Generate a grid of x and y values for the interpolated data
                x_new = np.linspace(x.min(), x.max(), data.shape[1] * interpolation_factor)
                y_new = np.linspace(y.min(), y.max(), data.shape[0] * interpolation_factor)

                # Interpolate the data to the new grid
                interpolated_data = f(x_new, y_new)

                # Set up the blur kernel
                kernel_size = 20
                kernel = np.ones((kernel_size, kernel_size), np.float32) / (kernel_size * kernel_size)

                # Apply image blur
                blur = cv.filter2D(np.flipud(interpolated_data), -1, kernel)

                # Create output directory if it does not exist
                if not os.path.exists(output_path):
                    os.makedirs(output_path)

                # Save the plot as a PNG file
                plt.imsave(new_output_path, blur, cmap='gray', dpi=300)

        else:
            print("Variable data has an unsupported shape")
            UnityEngine.Debug.Log("Variable data has an unsupported shape: " + variable_name)

except Exception as e:
    error_msg = f"Error processing {nc_path}: {str(e)}"
    print(error_msg)
    UnityEngine.Debug.LogError(error_msg)
