﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.Networking;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;

namespace Visualization
{
    public class CloudManager : MonoBehaviour
    {
        //More efficient to store these as static variables.
        private static readonly int ColorMapMin = Shader.PropertyToID("_ColorMapMin");
        private static readonly int ColorMapMax = Shader.PropertyToID("_ColorMapMax");
        private static readonly int ColorMapAlpha = Shader.PropertyToID("_ColorMapAlpha");
        private static readonly int TerrainCurvature = Shader.PropertyToID("_Terrain_Curvature");
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");
        private static readonly int Heightmap = Shader.PropertyToID("_TerrainHeightmap");

        
        private readonly List<Renderer> _cloudRenderers = new();

        public double baseElevation;
        
        private List<Texture2D> _cloudImages = new();
        public int MapCount => _cloudImages.Count;

        private int _index;
        
        
        public void Initialize( List<Texture2D> cloudImages, Texture2D heightMap, int size, double elevation)
        {
            _cloudImages = cloudImages;
            baseElevation = elevation;

            LODGroup lodGroup = gameObject.GetComponent<LODGroup>();
            lodGroup.size = size;
            
            foreach (LOD lod in lodGroup.GetLODs())
            {
                foreach (Renderer ren in lod.renderers)
                {
                    _cloudRenderers.Add(ren);
                    ren.material.SetTexture(ColorMapMin, cloudImages[_index]);
                    ren.material.SetTexture(ColorMapMax, cloudImages[_index + 1]);
                    ren.material.SetTexture(Heightmap, heightMap);
                }
            }
        }


        /* Changes opacity for the clouds-shader */
        public void ChangeOpacity(float value)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(Opacity, value);
            }
        }

        
        /* Updates the time-alpha in the clouds-material, interpolating between the two loaded textures*/
        public void ChangeTimeStep(float time)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetFloat(ColorMapAlpha, time % 1);
            }
        }
        
        
        /* Changes the physical height of the clouds in the scene, interpolating the heightmap more when reaching the baseElevation*/
        public void ChangeCurvatureByHeight(float value)
        {
            foreach (Renderer ren in _cloudRenderers)
            {
                // To create a steeper curve.
                if ( value >= 0.1f){
                    value*= 1.2f;
                }
                ren.material.SetFloat(TerrainCurvature, Mathf.Clamp01(1-(value+0.25f / 1)));
            }
        }
        
        
        public void UpdateTime(int steps)
        {
            int newIndex = _index + steps;

            if (newIndex < 0 || newIndex + 1 > _cloudImages.Count)
                throw new System.IndexOutOfRangeException("Index out of range");

            _index = newIndex;
            
            Debug.Log("Update time steps: " + steps);
            
            foreach (Renderer ren in _cloudRenderers)
            {
                ren.material.SetTexture(ColorMapMin, _cloudImages[_index]);
                ren.material.SetTexture(ColorMapMax, _cloudImages[_index + 1]);
            }
        }
    }
}