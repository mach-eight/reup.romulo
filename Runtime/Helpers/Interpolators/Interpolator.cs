using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface Interpolator<T>
    {
        public void DefineOriginAndTarget(Vector3 originalPostion, T targetPostion);
        public Vector3 Interpolate(Vector3 currentValue);
    }
}
