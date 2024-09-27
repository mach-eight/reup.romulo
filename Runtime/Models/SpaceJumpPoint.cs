using ReupVirtualTwin.modelInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.models
{
    public class SpaceJumpPoint : MonoBehaviour, ISpaceJumpPoint
    {
        public string _spaceName = "Unnamed space";
        public string spaceName { get => _spaceName; set => _spaceName = value; }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
