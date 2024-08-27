using UnityEngine;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface IMaterialScaler
    {
        public void AdjustUVScaleToDimensions(GameObject obj, Vector2 dimensionsInM);
    }
}
