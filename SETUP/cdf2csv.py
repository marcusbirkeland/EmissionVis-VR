import xarray as xr

nc = xr.open_dataset('bergen.nc')
nc["wspeed_10m*_xy"].to_dataframe().to_csv('wspeed.csv')
nc["rad_net*_xy"].to_dataframe().to_csv('rad.csv')
nc.close()

nc = xr.open_dataset('bergen_buildings.nc')
nc["buildings_2d"].to_dataframe().to_csv('buildings.csv')

nc.close()

print("Succesfully created CSV files!")
