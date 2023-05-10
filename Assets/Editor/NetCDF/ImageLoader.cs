using System.Collections.Generic;
using System.Linq;
using Editor.Utilities;
using UnityEngine;

namespace Editor.NetCDF
{
    /// <summary>
    /// Static class responsible for retrieving the images generated from netCDF data.
    /// </summary>
    public static class ImageLoader
    {
        /// <summary>
        /// Loads the cloud images associated with the specified map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which to load cloud images.</param>
        /// <returns>A list of <see cref="Texture2D"/> objects containing the loaded cloud images, sorted by their timestamps.</returns>
        public static List<Texture2D> GetCloudImages(string mapName)
        {
            Texture2D[] textures = Resources.LoadAll<Texture2D>($"{FilepathSettings.DataFilesFolderName}/{mapName}/WindSpeed");

            Debug.Log($"Found {textures.Length} wind speed images");
            
            return textures.OrderBy(t => t.name).ToList();
        }
        
        
        /// <summary>
        /// Loads heightMap image associated with the specified map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which to load the heightMap.</param>
        /// <returns> A <see cref="Texture2D"/> containing the heightmap data in grayscale.</returns>
        public static Texture2D GetHeightMapImg(string mapName)
        {
            string path = $"{FilepathSettings.DataFilesFolderName}/{mapName}/HeightMap/heightMap";
            Texture2D img = Resources.Load<Texture2D>($"{FilepathSettings.DataFilesFolderName}/{mapName}/HeightMap/heightMap");

            if (img == null)
            {
                Debug.LogError("Texture not found in Resources at: " + path);
            }

            return img;
        }


        /// <summary>
        /// Loads the radiation images associated with the specified map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which to load cloud images</param>
        /// <returns>A list of <see cref="Texture2D"/> objects containing the loaded radiation images.</returns>
        public static List<Texture2D> GetRadiationImages(string mapName)
        {
            Texture2D[] textures = Resources.LoadAll<Texture2D>($"{FilepathSettings.DataFilesFolderName}/{mapName}/Radiation");

            Debug.Log($"Found {textures.Length} radiation images");

            return textures.ToList();
        }
    }
}