using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapUI
{
    public class EmissionVisibilityScript : MonoBehaviour
    {
        private readonly List<Material> _currentMats = new();
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");

        // Start is called before the first frame update
        private void Start()
        {
            LOD[] lods = gameObject.GetComponent<LODGroup>().GetLODs();
            foreach (LOD lod in lods)
            {
                foreach (Renderer ren in lod.renderers)
                {
                    _currentMats.Add(ren.material);
                }
            }
        }

        private void ChangeAlpha(float alphaVal)
        {
            // NOTE: Ideally, only the current loaded LOD would have to be altered. 
            // There is no clear way to extract this information.
            foreach (Material mat in _currentMats)
            {
                mat.SetFloat(Opacity, alphaVal);
            }
        }

        public void ChangeAlphaOnValueChange(Slider slider)
        {
            ChangeAlpha(slider.value);
        }
    }
}
