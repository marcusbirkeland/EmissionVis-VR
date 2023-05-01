using UnityEngine;

namespace Visualization
{
    public static class RadiationManager
    {
        private static readonly int RadiationMap = Shader.PropertyToID("_RadiationMap");
        private static readonly int Heightmap = Shader.PropertyToID("_Heightmap");

        public static void SetRadiationImages(GameObject radiation, Texture2D radiationImage, Texture2D heightMapImage)
        {
            LOD[] lods = radiation.GetComponent<LODGroup>().GetLODs();
            foreach (LOD lod in lods)
            {
                foreach (Renderer ren in lod.renderers)
                {
                    ren.material.SetTexture(RadiationMap, radiationImage);
                    ren.material.SetTexture(Heightmap, heightMapImage);
                }
            }
        }
    }
}