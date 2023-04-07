using UnityEngine;

namespace Cloud
{
    public readonly struct CloudMap
    {
        public readonly Texture2D Texture;
        public readonly int Time;

        public CloudMap(Texture2D tex, int t){
            Texture = tex;
            Time = t;
        }
    }
}