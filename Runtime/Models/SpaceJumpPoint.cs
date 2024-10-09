using System;
using ReupVirtualTwin.modelInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.models
{
    public class SpaceJumpPoint : MonoBehaviour, ISpaceJumpPoint
    {
        public string _spaceName = "Unnamed space";
        public string spaceName { get => _spaceName; set => _spaceName = value; }
        public string _id = "";
        public string id { get => _id; set => _id = value; }

        public void Awake()
        {
            if (string.IsNullOrEmpty(id.Trim()))
            {
                id = Guid.NewGuid().ToString();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
