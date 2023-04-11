using System.IO;
using UnityEngine;

namespace Editor.VisualizationSpawner
{
    public class RadiationSpawner
    {
        //private readonly AttributeDataGetter.FileAttributes _selectedCdfAttributes;
        private readonly string _imgDataPath;
        private readonly string _cloudPrefabName;
        private readonly string _cloudManagerPrefabName;
        private readonly GameObject _map;
        private readonly float _rotationAngle;
        
        
        public RadiationSpawner(string imageFolderPath, string attributesFilePath, string cdfFilePath, GameObject map, string radiationPrefabName, float rotationAngle)
        {
            
        }

        public void SpawnAndSetupRadiation()
        {
            if (!DataFilesExist(_imgDataPath))
            {
                Debug.LogError($"The directory '{_imgDataPath}' does not exist.");
                return;
            }
        }

        private void CreateRadiationHolder()
        {
            
        }

        private void SpawnRadiation()
        {
            
        }

        private void DeletePreviousRadiation()
        {
            
        }

        private bool DataFilesExist(string dataPath)
        {
            if (!Directory.Exists(dataPath))
            {
                return false;
            }

            string[] pngFiles = Directory.GetFiles(dataPath, "*.png", SearchOption.TopDirectoryOnly);

            return pngFiles.Length > 0;
        }
    }
}