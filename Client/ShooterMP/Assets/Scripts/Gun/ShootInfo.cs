using System;

namespace ShooterMP.Gun
{
    [Serializable]
    public struct ShootInfo
    {
        public string key;
        
        public float pX;
        public float pY;
        public float pZ;

        public float dX;
        public float dY;
        public float dZ;
    }
}
