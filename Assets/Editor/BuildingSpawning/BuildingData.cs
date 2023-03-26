using System;

namespace Editor.BuildingSpawning
{
    [Serializable]
    public struct BuildingData
    {
        public double x;
        public double y;

        public BuildingData(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}