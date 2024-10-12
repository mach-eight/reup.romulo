using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class RayUtils
    {
        public static Vector3 GetHitPoint(Ray ray)
        {
            RaycastHit hit;
            if (CastRay(ray, out hit))
            {
                GameObject obj = hit.collider.gameObject;
                if (obj)
                {
                    return hit.point;
                }
            }
            return ProjectRayToHeight(ray, 0);
        }

        public static bool CastRay(Ray ray, out RaycastHit hit)
        {
            return Physics.Raycast(ray, out hit, Mathf.Infinity);
        }

        public static Vector3 ProjectRayToHeight(Ray ray, float height)
        {
            Vector3 origin = ray.origin;
            Vector3 direction = ray.direction;
            float i = (height - origin.y) / direction.y;
            return origin + i * direction;
        }
    }

}