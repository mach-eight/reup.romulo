using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class LayerMaskUtils
    {
        public static int GetLayerMaskById(int layerId)
        {
            return 1 << layerId;
        }

    }
}