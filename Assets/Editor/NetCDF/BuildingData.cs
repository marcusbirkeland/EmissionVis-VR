using System;

namespace Editor.NetCDF
{
    [Serializable]
    public readonly struct BuildingData
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Altitude;

        public BuildingData(double x, double y, double altitude)
        {
            X = x;
            Y = y;
            Altitude = altitude;
        }
    }
}