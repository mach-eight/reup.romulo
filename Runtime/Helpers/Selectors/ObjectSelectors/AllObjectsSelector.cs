using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class AllObjectsSelector : ObjectSelector
    {
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            return obj;
        }
    }
}