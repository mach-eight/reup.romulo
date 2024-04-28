using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class FractionalEaseInOutSpeedHandler : SpeedHandler
    {
        readonly float totalDistance;
        readonly float maxSpeedInMetersPerSecond;
        readonly float minSpeedInMetersPerSecond;
        readonly float fractionToMaxSpeed;
        readonly float fractionToMinSpeed;

        public FractionalEaseInOutSpeedHandler(float distance, float maxSpeed, float minSpeed, float fracToMax, float fracToMin)
        {
            totalDistance = distance;
            maxSpeedInMetersPerSecond = maxSpeed;
            minSpeedInMetersPerSecond = minSpeed;
            fractionToMaxSpeed = fracToMax;
            fractionToMinSpeed = fracToMin;
        }

        public float GetSpeedInMetersPerSecond(float traveledDistance)
        {
            var travelFraction = traveledDistance / totalDistance;
            if (travelFraction < fractionToMaxSpeed)
            {
                float growingSlope = (maxSpeedInMetersPerSecond - minSpeedInMetersPerSecond) / fractionToMaxSpeed;
                float growingSpeed = growingSlope * travelFraction  + minSpeedInMetersPerSecond;
                return growingSpeed;
            }
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
