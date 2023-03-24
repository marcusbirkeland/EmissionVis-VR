import pyproj

def convert_lat_lon_to_3857(latitude, longitude, source_epsg):
    source_proj = pyproj.Proj(f'epsg:{source_epsg}')
    target_proj = pyproj.Proj('epsg:3857')

    x, y = pyproj.transform(source_proj, target_proj, longitude, latitude)
    return x, y


la, lo = convert_lat_lon_to_3857(60.35954907032411, 5.314180944287559, 32632)

print(la, lo)