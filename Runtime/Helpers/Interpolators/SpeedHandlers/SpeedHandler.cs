using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface SpeedHandler
    {
        public float GetSpeedInMetersPerSecond(float traveledDistance);
    }
}
