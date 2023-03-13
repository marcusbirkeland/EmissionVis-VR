import netCDF4
import json

#input_paths = r"C:\Users\Lars\Downloads\bergen_24_summer_L_av_xy.003.nc$C:\Users\Lars\Downloads\bergen_24_summer_L_topo_surf.003.nc$C:\Users\Lars\Downloads".split('$')

input_paths = __name__.split('$')
output = {}

#Get data from all files
for i in range(len(input_paths) - 1):
    path = input_paths[i]

    outputvar = path.split('\\')[-1]
    output[outputvar] = []

    try:
        with netCDF4.Dataset(path, "r") as dataset:
            for var in dataset.variables:
                output[outputvar].append(var)
    except:
        print(f"Error reading {path}")


#Write data to outputfile
with open(input_paths[len(input_paths) - 1] + "\\variables.json", "w") as file:
    json.dump(output, file, indent=2)