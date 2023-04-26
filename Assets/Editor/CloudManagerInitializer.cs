using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Editor
{
    public static class CloudManagerInitializer
    {
        public static List<Texture2D> GetImages(string mapName)
        {
            Object[] loadedTextures = Resources.LoadAll($"MapData/{mapName}/WindSpeed", typeof(Texture2D));

            List<Texture2D> textures = loadedTextures.Select(obj => obj as Texture2D).ToList();

            Debug.Log($"Found {textures.Count} wind speed images");
            
            return textures.OrderBy(t => t.name).ToList();
        }
    }
}