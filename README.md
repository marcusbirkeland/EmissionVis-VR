# EmissionVis-VR
Link to GitHub repo: https://github.com/marcusbirkeland/EmissionVis-VR

Unity VR project to visualize real-world locations with climate-data.

![tutorial-and-miniature](https://github.com/marcusbirkeland/EmissionVis-VR/assets/46761308/3e737ab9-2fc8-4b74-85aa-3350847e3f56)

## Installation and setup

1. Download and install Unity Hub from https://unity.com/download

2. Sign into or create a Unity account

3. When prompted to install the newest version of the Unity Editor, press "Skip installation"

4. Clone this repository and store it somewhere on your computer. 

5. Inside the Unity Hub, go to Projects > Open, and open the root folder of the repository (EmissionVis-VR/) 

6. You will be prompted to install the Unity Editor version 2021.3.18f1. Make sure this version is selected and click "Install" 

7. When asked to install modules select "Android Build Support", "OpenJDK" and "Android SDK & NDK Tools". Press continue, agree to the following terms and click "Install"

8. Note that the project does not work yet, because it relies on an external package that needs to be installed first. Please follow the steps listed below. 



### Install and setup ArcGIS package

1. Open the project from Unity Hub. You will get an error indicating that the ArcGIS package is missing. Press "Continue" and "Ignore"

2. You will need to sign up for an ArcGIS developer account (this is free), download the package, and get an API key, as described 
  in steps 1, 2 and 4 at https://developers.arcgis.com/unity/get-started/

3. Add the package to the project:
  - In the open Unity project, press Window > Package Manager
  - Press the "+" button in the upper left corner
  - Press "Add package from tarball...". Select the ArcGIS plugin you downloaded

![image](https://user-images.githubusercontent.com/36818485/216335467-539156c4-b918-49f3-b8aa-2059c47de3c2.png)
  
5. Wait for the package to install, then restart Unity.
  
6. Add your ArcGIS API-key. (In order to load maps, you need to authenticate the plugin with a key.)
  - In Unity, select "ArcGIS Maps SDK" from the toolbar
  - Go to the "Auth" section and enter your API-key
  - Make sure to **not** share this key publicly

![image](https://user-images.githubusercontent.com/36818485/216334722-6dec2bb1-e29d-43c0-b6c7-f15fb6917493.png)

 Here's how to get an arcGIS [API-key](https://developers.arcgis.com/documentation/mapping-apis-and-services/security/api-keys/)
 
 
 ### Setup API key for Bing Maps 

1. You will need to sign up for a Bing Maps developer account and create an API key as described in step 2 at https://github.com/microsoft/MapsSDK-Unity/wiki/Getting-Started

2. Create a resource file to store the key
  - In the Assets/Resources directory of the project, create a file named MapSessionConfig.txt
  - Copy the developer key into MapSessionConfig.txt
  - Make sure to **not** share this key publicly

A final note: Python should be installed on the computer running the project to ensure some Python packages get installed automatically  

The project should now be ready to use.

## Using the Sintef editor-tool

1. From the toolbar, select Sintef > Generate scenes 

2. You will need to add all the netCDF files needed to visualize the desired climate data before clicking "Get data"

NOTE: it's important that all files contain data covering the same exact location, and have the same size. I.e. you cannot have one dataset covering Bergen and one covering Oslo, as one relies on the other.
![netCFD-selection](https://github.com/marcusbirkeland/EmissionVis-VR/assets/46761308/a760bf91-e890-4fda-887d-ac02b5082b09)

3. Select which field names in the netCDF files correspond to building data, a heightmap/elevation data, wind speed (or any climate data), and radiation data
![Scene-gen](https://github.com/marcusbirkeland/EmissionVis-VR/assets/46761308/b4764fbf-c10e-47da-954a-c7a175753166)

4. After clicking "Generate scenes" the new scenes are added to the Assets folder, these are the ones used when building the application to a VR-headset

## Setup of Meta Quest headset

1. This project only supports building to the Meta Quest 1 and Meta Quest 2 headsets

2. Carefully follow the steps listed at https://developer.oculus.com/documentation/unity/unity-env-device-setup/#headset-setup, specifically the "Test Connection through USB" section

## Build application

1. From the toolbar go to File > Build settings...
2. Switch platform to Android
3. Drag the previously generated Miniature scene from the Assets/ folder into the area called "Scenes In Build"
4. Drag the Full Scale scene underneath the Miniature scene, such that they are ordered like in the screenshot below ("DEMO" will be substituted with the name you provided when generating the scenes)
![build-scene-order](https://github.com/marcusbirkeland/EmissionVis-VR/assets/46761308/a701e7e8-a5e5-480d-9d84-8b33aa3e5431)
5. Confirm that your Meta Quest headset is set as "Run Device" and click "Build and Run"

The application should start automatically on your headset when building has completed!



# Credits

icons from: 

<a href="https://www.flaticon.com/free-icons/augmented-reality" title="augmented reality icons">Augmented reality icons created by Ilham Fitrotul Hayat - Flaticon</a>
<a href="https://www.flaticon.com/free-icons/virtual-tour" title="virtual tour icons">Virtual tour icons created by Freepik - Flaticon</a>
