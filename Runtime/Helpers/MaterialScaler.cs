using ReupVirtualTwin.helperInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class MaterialScaler : IMaterialScaler
    {
        public void AdjustUVScaleToDimensions(GameObject obj, Vector2 dimensionsInMilimeters)
        {
            UvUtils.AdjustUVScaleToDimensions(obj, dimensionsInMilimeters);
        }
    }
}
