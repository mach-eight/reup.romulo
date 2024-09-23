using UnityEngine;
using ReupVirtualTwin.enums;
using System.Collections.Generic;

namespace ReupVirtualTwin.helpers
{
    public class MoveToHitSelector : RayCastHitSelector
    {
        private HashSet<string> ignoreTags = new HashSet<string>
        {
            TagsEnum.trigger,
            TagsEnum.materialSelection
        };
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            if (ignoreTags.Contains(obj.tag))
            {
                return null;
            }
            return obj;
        }
    }
}
