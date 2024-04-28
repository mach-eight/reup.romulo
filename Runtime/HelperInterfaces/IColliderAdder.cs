using UnityEngine;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface IColliderAdder
    {
        public void AddColliderToObject(GameObject obj);
        public void AddCollidersToTree(GameObject tree);
    }
}
