using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class ViewportUtils
    {
        public static int MinViewportDimension(Camera camera)
        {
            return Mathf.Min(camera.pixelWidth, camera.pixelHeight);
        }
    }
}
