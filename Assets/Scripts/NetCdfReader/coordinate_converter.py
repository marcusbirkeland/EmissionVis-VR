from pyproj import Proj, Transformer

def convert_utm_to_latlon(x, y, epsg_code):
    # Create UTM and WGS84 coordinate system objects
    utm_proj = Proj(f"epsg:{epsg_code}")
    wgs84_proj = Proj("epsg:4326")

    # Create the transformer
    transformer = Transformer.from_proj(utm_proj, wgs84_proj)

    # Perform the transformation
    lon, lat = transformer.transform(x, y)

    return lat, lon

# Example usage
x = 296720.102
y = 6697140.253
epsg_code = 32632

lat, lon = convert_utm_to_latlon(x, y, epsg_code)
print(f"Latitude: {lat}, Longitude: {lon}")


print(float(lat)== 60.35954907032411)
print(float(lon) == 5.314180944287559)