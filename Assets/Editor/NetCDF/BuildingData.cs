using System;

namespace Editor.NetCDF
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