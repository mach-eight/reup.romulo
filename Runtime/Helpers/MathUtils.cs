using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class MathUtils
    {
        public static float NormalizeAngle(float angle)
        {
            return -(((-angle + 180) % 360 + 360) % 360) + 180;
        }

        public static float NormalizeAngleRad(float angle)
        {
            float pi = Mathf.PI;
            return -(((-angle + pi) % (2 * pi) + (2 * pi)) % (2 * pi)) + pi;
        }
    }
}