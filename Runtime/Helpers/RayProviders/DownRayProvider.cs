using UnityEngine;
using ReupVirtualTwin.helperInterfaces;


namespace ReupVirtualTwin.helpers
{
    public class DownRayProvider : MonoBehaviour, IRayProvider
    {
        public Ray GetRay()
        {
            return new Ray(transform.position, Vector3.down);
        }
    }
}