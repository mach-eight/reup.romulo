using ReupVirtualTwin.helperInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class ColliderAdder : IColliderAdder
    {
        public void AddCollidersToTree(GameObject tree)
        {
            AddColliderToObject(tree);
            foreach (Transform child in tree.transform)
            {
                AddCollidersToTree(child.gameObject);
            }
        }

        public void AddColliderToObject(GameObject obj)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider == null)
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    var meshColider = obj.AddComponent<MeshCollider>();
                    meshColider.convex = false;
                }
            }
        }
    }
}
