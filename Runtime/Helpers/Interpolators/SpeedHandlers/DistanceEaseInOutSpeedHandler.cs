using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class DistanceEaseInOutSpeedHandler : SpeedHandler
    {
        readonly float totalDistance;
        readonly float maxSpeedInMetersPerSecond;
        readonly float minSpeedInMetersPerSecond;
        readonly float distanceToMaxSpeed;
        readonly float distanceToMinSpeed;
        readonly float changeSlopeFraction;

        public DistanceEaseInOutSpeedHandler(float distance, float maxSpeed, float minSpeed, float distanceToMax, float distanceToMin)
        {
            totalDistance = distance;
            maxSpeedInMetersPerSecond = maxSpeed;
            minSpeedInMetersPerSecond = minSpeed;
            distanceToMaxSpeed = distanceToMax;
            distanceToMinSpeed = distanceToMin;
            changeSlopeFraction = distanceToMaxSpeed / (distanceToMaxSpeed + distanceToMinSpeed);
        }

        public float GetSpeedInMetersPerSecond(float traveledDistance)
        {
            float speed = GetSpeedNoMax(traveledDistance);
            return Mathf.Min(speed, maxSpeedInMetersPerSecond);
        }
        private float GetSpeedNoMax(float traveledDistance)
        {
            var travelFraction = traveledDistance / totalDistance;
            if (travelFraction < changeSlopeFraction)
            {
                float growingSlope = (maxSpeedInMetersPerSecond - minSpeedInMetersPerSecond) / distanceToMaxSpeed;
                float growingSpeed = growingSlope * traveledDistance  + minSpeedInMetersPerSecond;
                return growingSpeed;
            }
            float decreasingSlope = (minSpeedInMetersPerSecond - maxSpeedInMetersPerSecond) / distanceToMinSpeed;
            float decreasingSpeed = decreasingSlope * (traveledDistance - totalDistance) + minSpeedInMetersPerSecond; 
            return Mathf.Max(decreasingSpeed, minSpeedInMetersPerSecond);
        }
    }
}
