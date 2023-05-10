# EmissionVis-VR
Unity VR project to visualize real-world locations with climate-data.

![FS](https://github.com/marcusbirkeland/EmissionVis-VR/assets/46761308/80f5eb22-8cc2-49bf-9491-35f1d2dd7374)

## How to set up Unity

1. Download and install Unity Hub from https://unity.com/download
2. Sign into or create a Unity account
3. When promptet to install the newest version of the Unity Editor, press "Skip installation"
4. Clone this repository and store it somewhere on your computer. 
5. Inside the Unity Hub, go to Projects > Open, and open the root folder (EmissionVis-VR/) 

## How to set up this project

1. Clone this repository

2. Open the root folder as a Unity project. You will get an error indicating some packages are missing. Press Continue

3. Download the ArcGIS Unity plugin from here: [ArcGIS Unity](https://developers.arcgis.com/downloads/#unity)

4. Install the package:
  - In the open Unity project, press Window->Package Manager.
  - Press the "+" button in the upper left corner.
  - Press "Add package from tarball...". Select the ArcGIS plugin you downloaded.

![image](https://user-images.githubusercontent.com/36818485/216335467-539156c4-b918-49f3-b8aa-2059c47de3c2.png)
  
5. Wait for the package to install, then restart Unity.
  
6. Add your ArcGIS API-key. (In order to load maps, you need to authenticate the plugin with a key.)

  - In Unity, select "ArcGIS Maps SDK" from the toolbar. 
  - Go to the "Auth" section and enter your API-key.

![image](https://user-images.githubusercontent.com/36818485/216334722-6dec2bb1-e29d-43c0-b6c7-f15fb6917493.png)

 Here's how to get an arcGIS [API-key](https://developers.arcgis.com/documentation/mapping-apis-and-services/security/api-keys/)

# Credits

icons from: 

<a href="https://www.flaticon.com/free-icons/augmented-reality" title="augmented reality icons">Augmented reality icons created by Ilham Fitrotul Hayat - Flaticon</a>
<a href="https://www.flaticon.com/free-icons/virtual-tour" title="virtual tour icons">Virtual tour icons created by Freepik - Flaticon</a>
