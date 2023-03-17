import matplotlib
matplotlib.use('Agg')

import netCDF4
import os
import numpy as np
import cv2 as cv
from matplotlib import pyplot as plt
from netCDF4 import Dataset

#input_fields = __name__.split('$')

#nc_path = input_fields[0]
#variable_name = input_fields[1]
#output_path = input_fields[2]
#interpolation_factor = int(input_fields[3])

nc_path = r"C:\Users\Lars\Downloads\bergen_24_summer_L_topo_surf.003.nc"
variable_name = "zt"
output_path = r"C:\Users\Lars\Downloads\topo"
blur_factor = 3

def apply_gaussian_blur_and_save_image(data, output_path, blur_factor):
    blur_factor = blur_factor + 1 if blur_factor % 2 == 0 else blur_factor
    blurred_data = cv.GaussianBlur(data, (blur_factor, blur_factor), 0)
    plt.imsave(output_path, blurred_data, cmap='gray', dpi=300)


# Check the shape of the variable data and create either a single png file or multiple png files
with netCDF4.Dataset(nc_path, "r") as nc_file:

    variable_data = nc_file.variables[variable_name][:]

    # Set up the blur kernel
    kernel_size = 25
    kernel = np.ones((kernel_size, kernel_size), np.float32) / (kernel_size * kernel_size)

    if len(variable_data.shape) == 2:

        dir_path = os.path.dirname(output_path)
        if not os.path.exists(dir_path):
            os.makedirs(dir_path)

        # Create a single PNG file for the variable data
        output_path = output_path + ".png"

        # Get the variable
        nc_var = nc_file.variables[variable_name]

        # Read the data from the NetCDF file
        data = nc_var[:, :]

        apply_gaussian_blur_and_save_image(data, output_path, blur_factor)


    elif len(variable_data.shape) == 4:
        
        output_path += "\\"
        if not os.path.exists(output_path):
            os.makedirs(output_path)

        for i in range(variable_data.shape[0]):
            time_value = int(nc_file.variables['time'][i])
            new_output_path = output_path + "{}.png".format(time_value)
            data = variable_data[i, 0]

            apply_gaussian_blur_and_save_image(data, new_output_path, blur_factor)

    else:
        print("Variable data has an unsupported shape")
