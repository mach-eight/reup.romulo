using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class AddGizmo : MonoBehaviour
    {
        public Color color = Color.black;
        public float radius = 0.05f;
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
