using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Editor.NetCDF
{
    /// <summary>
    /// Static class responsible for loading cloud images from a folder within the Resources folder.
    /// </summary>
    public static class CloudImageLoader
    {
        /// <summary>
        /// Loads the cloud images associated with the specified map name.
        /// </summary>
        /// <param name="mapName">The name of the map for which to load cloud images.</param>
        /// <returns>A list of <see cref="Texture2D"/> objects containing the loaded cloud images, sorted by their timestamps.</returns>
        public static List<Texture2D> GetImages(string mapName)
        {
            Object[] loadedTextures = Resources.LoadAll($"MapData/{mapName}/WindSpeed", typeof(Texture2D));

            List<Texture2D> textures = loadedTextures.Select(obj => obj as Texture2D).ToList();

            Debug.Log($"Found {textures.Count} wind speed images");
            
            return textures.OrderBy(t => t.name).ToList();
        }
    }
}