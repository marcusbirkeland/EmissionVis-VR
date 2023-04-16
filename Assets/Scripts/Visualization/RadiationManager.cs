using UnityEngine;

namespace Visualization
{
    public class RadiationManager : MonoBehaviour
    {
        [HideInInspector]
        public Texture2D radiationImage;

        [HideInInspector]
        public Texture2D heightMapImg;
        
        private static readonly int RadiationMap = Shader.PropertyToID("_RadiationMap");
        private static readonly int Heightmap = Shader.PropertyToID("_Heightmap");


        private void Start()
        {
            LOD[] lods = gameObject.GetComponent<LODGroup>().GetLODs();
            foreach (LOD lod in lods)
            {
                foreach (Renderer ren in lod.renderers)
                {
                    ren.material.SetTexture(RadiationMap, radiationImage);
                    ren.material.SetTexture(Heightmap, heightMapImg);
                }
            }
        }
    }
}