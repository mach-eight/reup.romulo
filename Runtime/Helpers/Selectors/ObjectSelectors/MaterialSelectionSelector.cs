using UnityEngine;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.helpers
{
    public class MaterialSelectionSelector : ObjectSelector
    {
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            if (obj.CompareTag(TagsEnum.materialSelection))
            {
                return obj;
            }
            return null;
        }
    }
}
