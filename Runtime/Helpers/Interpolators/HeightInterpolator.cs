using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class HeightInterpolator : Interpolator<float>
    {
        float origin;
        float target;
        float MAX_SPEED_IN_METERS_PER_SECOND = 3f;
        float MIN_SPEED_IN_METERS_PER_SECOND = 1f;
        float DISTANCE_TO_MAX_SPEED = 0.05f;
        float DISTANCE_TO_MIN_SPEED = 0.1f;
        float MAX_DELTA = 0.1f;
        SpeedHandler speedHandler;

        public void DefineOriginAndTarget(Vector3 originalPostion, float targetHeight)
        {
            origin = originalPostion.y;
            target = targetHeight;
            var totalDistance = Mathf.Abs(target - origin);
            speedHandler = new DistanceEaseInOutSpeedHandler(totalDistance,
                MAX_SPEED_IN_METERS_PER_SECOND,
                MIN_SPEED_IN_METERS_PER_SECOND,
                DISTANCE_TO_MAX_SPEED,
                DISTANCE_TO_MIN_SPEED);
        }

        public Vector3 Interpolate(Vector3 currentPosition)
        {
            float currentHeight = currentPosition.y;
            float traveledDistance = Mathf.Abs(currentHeight - origin);
            float speed = speedHandler.GetSpeedInMetersPerSecond(traveledDistance);
            int direction = getDirection(currentHeight);
            float delta = direction * speed * Time.deltaTime;
            if (Mathf.Abs(delta) > MAX_DELTA)
            {
                delta = MAX_DELTA * direction;
            }
            currentHeight += delta;
            var newPosition = new Vector3(currentPosition.x, currentHeight, currentPosition.z);
            return newPosition;
        }

        private int getDirection(float currentHeight)
        {
            if ( currentHeight >= target)
            {
                return -1;
            }
            return 1;
        }
    }
}
