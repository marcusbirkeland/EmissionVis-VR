import xarray as xr
import json


nc = xr.open_dataset('bergen.nc')
wspeed = nc["wspeed_10m*_xy"].to_dataframe().to_json()
rad = nc["rad_net*_xy"].to_dataframe().to_json()
nc.close()

nc = xr.open_dataset('bergen_buildings.nc')
buildings = nc["buildings_2d"].to_dataframe().to_json()

nc.close()


with open('wspeed.json', 'w', encoding='utf-8') as f:
    json.dump(wspeed, f, ensure_ascii=False, indent=4)

with open('rad.json', 'w', encoding='utf-8') as f:
    json.dump(rad, f, ensure_ascii=False, indent=4)

with open('buildings.json', 'w', encoding='utf-8') as f:
    json.dump(buildings, f, ensure_ascii=False, indent=4)

print("Succesfully created JSON files!")
