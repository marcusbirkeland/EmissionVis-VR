# EmissionVis-VR
Unity VR project to visualize real-world locations with emission-data.

![image](https://user-images.githubusercontent.com/36818485/215627193-f9225754-4598-4695-9e13-27be4a93c961.png)


## How to set up

1. Clone this repository

2. Open the root folder as a Unity project. You will get an error indicating some packages are missing. Press Continue

3. Download the ArcGIS Unity plugin from here: [ArcGIS Unity](https://developers.arcgis.com/downloads/#unity)

4. Install the package:
  - In the open Unity project, press Window->Package Manager.
  - Press the "+" button in the upper left corner.
  - Press "Add package from tarball...". Select the ArcGIS plugin you downloaded.
  
5. Restart Unity
  
6. Add your ArcGIS API-key. (In order to load maps, you need to authenticate the plugin with a key.)

  - In Unity, select "ArcGIS Maps SDK" from the toolbar. 
  - Go to the "Auth" section and enter your API-key.

![image](https://user-images.githubusercontent.com/36818485/216334722-6dec2bb1-e29d-43c0-b6c7-f15fb6917493.png)

 Here's how to get an arcGIS [API-key](https://developers.arcgis.com/documentation/mapping-apis-and-services/security/api-keys/)

