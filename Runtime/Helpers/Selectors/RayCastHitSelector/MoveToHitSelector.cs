using UnityEngine;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.helpers
{
    public class MoveToHitSelector : RayCastHitSelector
    {
        private string[] ignoreTags = new string[]
        {
            TagsEnum.trigger,
            TagsEnum.materialSelection,
        };
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            foreach(string tag in ignoreTags)
            {
                if (obj.CompareTag(tag))
                {
                    return null;
                }
            }
            return obj;
        }
    }
}
