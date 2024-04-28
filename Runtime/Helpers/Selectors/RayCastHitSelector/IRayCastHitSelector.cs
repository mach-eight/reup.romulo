using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface IRayCastHitSelector
    {
        public RaycastHit? GetHit(Ray ray);
    }
}
