using ReupVirtualTwin.modelInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IIdAssignerController
    {
        public void AssignIdsToTree(GameObject tree, string parentTreeId = null);
        public void RemoveIdsFromTree(GameObject tree);
        public IUniqueIdentifier AssignIdToObject(GameObject obj, string objectId = null);
        public void RemoveIdFromObject(GameObject obj);
    }
}
