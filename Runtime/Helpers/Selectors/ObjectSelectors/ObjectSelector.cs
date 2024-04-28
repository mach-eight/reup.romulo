using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public abstract class ObjectSelector : Selector, IObjectSelector
    {
        public GameObject GetObject(Ray ray)
        {
            RaycastHit hit;
            if (CastRay(ray, out hit))
            {
                return GetSelectedObjectFromHitObject(hit.collider.gameObject);
            }
            return null;
        }


    }
}