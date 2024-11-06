using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class MathUtils
    {
        public static float NormalizeAngle(float angle)
        {
            return -(((-angle + 180) % 360 + 360) % 360) + 180;
        }

        public static float NormalizeTo360Degrees(float angle)
        {
            return ((angle % 360) + 360) % 360;
        }

        public static float NormalizeAngleRad(float angle)
        {
            float pi = Mathf.PI;
            return -(((-angle + pi) % (2 * pi) + (2 * pi)) % (2 * pi)) + pi;
        }

        public static float CalculateAngle(Quaternion a, Quaternion b)
        {
            // for some reason Quaternion.Angle(a, b) does not return angles when they are very small, so we had to calculate these angles manually ¯\_(ツ)_/¯
            float dotProduct = Quaternion.Dot(a, b);
            dotProduct = Mathf.Clamp(dotProduct, -1.0f, 1.0f);
            float angleInRadians = Mathf.Acos(dotProduct) * 2.0f;
            return angleInRadians * Mathf.Rad2Deg;
        }

    }
}