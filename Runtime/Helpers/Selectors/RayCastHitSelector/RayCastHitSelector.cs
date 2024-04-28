using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public abstract class RayCastHitSelector : Selector, IRayCastHitSelector
    {
        public RaycastHit? GetHit(Ray ray)
        {
            RaycastHit hit;
            if (CastRay(ray, out hit))
            {
                GameObject obj = hit.collider.gameObject;
                if (GetSelectedObjectFromHitObject(obj))
                {
                    return hit;
                }
            }
            return null;
        }

    }
}
