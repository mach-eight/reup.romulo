using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class SpacesInterpolator : Interpolator<Vector3>
    {
        Vector3 origin;
        Vector3 target;
        Vector3 direction;
        Vector3 currentPosition;
        float totalDistance;
        float MAX_SPEED_IN_METERS_PER_SECOND = 4f;
        float MIN_SPEED_IN_METERS_PER_SECOND = 1.5f;
        float FRACTION_TO_MAX_SPEED = 0.1f;
        float FRACTION_TO_SLOW_DOWN = 0.9f;
        SpeedHandler speedHandler;

        public void DefineOriginAndTarget(Vector3 originalPostion, Vector3 targetPostion)
        {
            origin = originalPostion;
            target = targetPostion;
            currentPosition = originalPostion;
            totalDistance = Vector3.Distance(origin, target);
            speedHandler = new FractionalEaseInOutSpeedHandler(totalDistance,
                MAX_SPEED_IN_METERS_PER_SECOND,
                MIN_SPEED_IN_METERS_PER_SECOND,
                FRACTION_TO_MAX_SPEED,
                FRACTION_TO_SLOW_DOWN);
        }

        public Vector3 Interpolate(Vector3 currentValue)
        {
            direction = Vector3.Normalize(target - currentValue);
            float traveledDistance = Vector3.Distance(currentValue, origin);
            float speed = speedHandler.GetSpeedInMetersPerSecond(traveledDistance);
            currentPosition += direction * speed * Time.deltaTime;
            return currentPosition;
        }
    }
}
