using UnityEngine;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.helpers
{
    public class TriggerSelector : ObjectSelector
    {
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            if (obj.tag == TagsEnum.trigger)
            {
                return obj;
            }
            return null;
        }
    }
}
