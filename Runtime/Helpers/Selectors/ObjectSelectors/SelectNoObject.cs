using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class SelectNoObject : ObjectSelector
    {
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            return null;
        }
    }
}
