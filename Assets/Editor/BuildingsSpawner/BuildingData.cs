using System;

namespace Editor.BuildingsSpawner
{
    [Serializable]
    public readonly struct BuildingData
    {
        public readonly double X;
        public readonly double Y;

        public BuildingData(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}