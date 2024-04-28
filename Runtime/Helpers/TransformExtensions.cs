using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class TransformExtensions
    {
        public static Vector3 GetTotalScale(this Transform transform)
        {
            Vector3 totalScale = Vector3.one;
            Transform currentTransform = transform;

            while (currentTransform != null)
            {
                totalScale = Vector3.Scale(totalScale, currentTransform.localScale);
                currentTransform = currentTransform.parent;
            }

            return totalScale;
        }
    }
}
