using System.Collections.Generic;
using UnityEngine;

namespace Visualization
{
    /// <summary>
    /// The CloudManager class handles the functionality of managing the appearance and properties of the clouds in the scene.
    /// </summary>
    public class CloudManager : MonoBehaviour
    {
        /// <summary>
        /// The minimum elevation for the cloud.
        /// Represents a number of meters above sea level.
        /// </summary>
        public double baseElevation;
        
        private int _index;
        
        // More efficient to store these as static variables.
        private static readonly int ColorMapMin = Shader.PropertyToID("_ColorMapMin");
        private static readonly int ColorMapMax = Shader.PropertyToID("_ColorMapMax");
        private static readonly int ColorMapAlpha = Shader.PropertyToID("_ColorMapAlpha");
        private static readonly int TerrainCurvature = Shader.PropertyToID("_Terrain_Curvature");
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");
        private static readonly int Heightmap = Shader.PropertyToID("_TerrainHeightmap");
        
        /// <summary>
        /// The number of timestamps in the list of shader textures.
        /// </summary>
        public int MapCount => cloudImages.Count;
        
        [SerializeField] 
        private List<Renderer> cloudRenderers = new();
        
        [SerializeField] 
        private List<Texture2D> cloudImages = new();
        

        /// <summary>
        /// Initializes the CloudManager with a set of cloud images, heightmap, size, and elevation.
        /// Essentially a constructor usable with <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <param name="newCloudImages">A list of cloud images.</param>
        /// <param name="heightMap">The heightmap texture.</param>
        /// <param name="size">The size of the LODGroup component.</param>
        /// <param name="elevation">The base elevation value.</param>
        public void Initialize(List<Texture2D> newCloudImages, Texture2D heightMap, int size, double elevation)
        {
            cloudImages = newCloudImages;
            baseElevation = elevation;

            LODGroup lodGroup = gameObject.GetComponent<LODGroup>();
            lodGroup.size = size;

            foreach (LOD lod in lodGroup.GetLODs())
            {
                foreach (Renderer ren in lod.renderers)
                {
                    cloudRenderers.Add(ren);

                    Material material = ren.sharedMaterial;
                    material.SetTexture(ColorMapMin, newCloudImages[_index]);
                    material.SetTexture(ColorMapMax, newCloudImages[_index + 1]);
                    material.SetTexture(Heightmap, heightMap);
                }
            }
        }

        
        /// <summary>
        /// Changes the opacity of every renderer in the cloud visualization.
        /// </summary>
        /// <param name="value">The opacity value (0 to 1) to set for the clouds.</param>
        public void ChangeOpacity(float value)
        {
            foreach (Renderer ren in cloudRenderers)
            {
                ren.material.SetFloat(Opacity, value);
            }
        }

        
        /// <summary>
        /// Updates the time-alpha in the clouds-material, interpolating between the two loaded textures.
        /// </summary>
        /// <param name="time">The time value representing the desired interpolation value between two textures.</param>
        public void ChangeTimeStep(float time)
        {
            foreach (Renderer ren in cloudRenderers)
            {
                ren.material.SetFloat(ColorMapAlpha, time % 1);
            }
        }

        

        /// <summary>
        /// Adjusts the degree to which the cloud visualization conforms to the terrain's contours based on its height above the ground.
        /// As the cloud gets closer to the ground, it adapts more to the terrain's shape, and vice versa.
        /// </summary>
        /// <param name="value">A float between 0 and 1 representing the relative height of the clouds as a percentage between the minimum and maximum height.</param>
        public void ChangeCurvatureByHeight(float value)
        {
            foreach (Renderer ren in cloudRenderers)
            {
                if (value >= 0.1f)
                {
                    value *= 1.2f;
                }

                ren.material.SetFloat(TerrainCurvature, Mathf.Clamp01(1 - (value + 0.25f / 1)));
            }
        }

        
        /// <summary>
        /// Updates the cloud textures to display the next or previous cloud texture based on the specified steps.
        /// </summary>
        /// <param name="steps">The number of steps to update the cloud texture. Positive values move forward in time, while negative values move backward in time.</param>
        public void UpdateTime(int steps)
        {
            int newIndex = _index + steps;

            if (newIndex < 0 || newIndex + 1 > cloudImages.Count)
                throw new System.IndexOutOfRangeException("Index out of range");

            _index = newIndex;

            Debug.Log("Update time steps: " + steps);

            foreach (Renderer ren in cloudRenderers)
            {
                ren.material.SetTexture(ColorMapMin, cloudImages[_index]);
                ren.material.SetTexture(ColorMapMax, cloudImages[_index + 1]);
            }
        }
    }
}
