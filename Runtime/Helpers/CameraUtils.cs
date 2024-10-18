using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class CameraUtils
    {
        public static float GetVerticalFov(Camera camera)
        {
            // Make sure the main camera component has "Field of View Axis" set to "Vertical"
            return camera.fieldOfView;
        }
        public static float GetHorizontalFov(Camera camera)
        {
            // Make sure the main camera component has "Field of View Axis" set to "Vertical"
            return 2 * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.aspect) * Mathf.Rad2Deg;
        }

        public static float GetTravelAngleFromViewPortCenterInRad(float relativeToViewPortPointerMovement, float fovDeg)
        {
            return MathUtils.NormalizeAngleRad(Mathf.Atan(2 * relativeToViewPortPointerMovement * Mathf.Tan(fovDeg * Mathf.Deg2Rad / 2)));
        }
    }

}