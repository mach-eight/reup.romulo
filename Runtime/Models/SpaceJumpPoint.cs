using UnityEngine;

namespace ReupVirtualTwin.models
{
    public class SpaceJumpPoint : MonoBehaviour
    {
        public string spaceName = "Unnamed space";

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
