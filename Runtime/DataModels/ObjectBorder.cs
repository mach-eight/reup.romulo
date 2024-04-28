using UnityEngine;

namespace ReupVirtualTwin.dataModels
{
    public struct ObjectBorder
    {
        public Vector3 maxBorders;
        public Vector3 minBorders;

        public CenteredObjectBorder TransformToCenterSize()
        {
            Vector3 center = new Vector3
            {
                x = (minBorders.x + maxBorders.x) / 2,
                y = (minBorders.y + maxBorders.y) / 2,
                z = (minBorders.z + maxBorders.z) / 2
            };
            Vector3 size = new Vector3
            {
                x = maxBorders.x - minBorders.x,
                y = maxBorders.y - minBorders.y,
                z = maxBorders.z - minBorders.z
            };
            return new CenteredObjectBorder
            {
                center = center,
                size = size
            };
        }
    }
    public struct CenteredObjectBorder
    {
        public Vector3 center;
        public Vector3 size;
    }

}
