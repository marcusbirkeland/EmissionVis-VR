using System;
using System.Collections.Generic;
using System.IO;

namespace Editor.BuildingsSpawner
{
    public static class BuildingSpawnerController
    {
        public static List<BuildingData> LoadBuildingData(string dataPath)
        {
            List<BuildingData> buildingDataList = new();

            using StreamReader streamReader = new(dataPath);

            long currentLine = 0;

            while (streamReader.Peek() >= 0)
            {
                float[] data = AssertDataFormat(streamReader.ReadLine(), currentLine);
                buildingDataList.Add(new BuildingData(data[1], data[0]));

                currentLine++;
            }

            return buildingDataList;
        }

        private static float[] AssertDataFormat(string data, long line)
        {
            string[] stringValues = data.Split(',');

            if (stringValues.Length != 2)
            {
                throw new ArgumentException(
                    $"Invalid data format at line: {line}. There should only be two columns of data values, but there are: {stringValues.Length}");
            }

            float[] floatArray = new float[2];

            for (int i = 0; i < stringValues.Length; i++)
            {
                if (!float.TryParse(stringValues[i], out floatArray[i]))
                {
                    throw new ArgumentException(
                        $"Invalid data format at line: {line}, and column: {i + 1}. Make sure the input data contains valid float values. Current value: {stringValues[i]}");
                }
            }

            return floatArray;
        }
    }
}