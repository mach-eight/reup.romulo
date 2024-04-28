using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class FractionalEaseOutSpeedHandler : SpeedHandler
    {
        readonly float totalDistance;
        readonly float maxSpeedInMetersPerSecond;
        readonly float minSpeedInMetersPerSecond;
        readonly float fractionToMinSpeed;

        public FractionalEaseOutSpeedHandler(float distance, float maxSpeed, float minSpeed, float fracToMin)
        {
            totalDistance = distance;
            maxSpeedInMetersPerSecond = maxSpeed;
            minSpeedInMetersPerSecond = minSpeed;
            fractionToMinSpeed = fracToMin;
        }

        public float GetSpeedInMetersPerSecond(float traveledDistance)
        {
            var travelFraction = traveledDistance / totalDistance;
            if (travelFraction < fractionToMinSpeed)
            {
                return maxSpeedInMetersPerSecond;
            }
            float decreasingSlope = (minSpeedInMetersPerSecond - maxSpeedInMetersPerSecond) / (1 - fractionToMinSpeed);
            float decreasingSpeed = decreasingSlope * (travelFraction - 1) + minSpeedInMetersPerSecond; 
            return Mathf.Max(decreasingSpeed, minSpeedInMetersPerSecond);
        }
    }
}
