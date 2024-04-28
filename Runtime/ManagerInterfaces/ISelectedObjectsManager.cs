using ReupVirtualTwin.dataModels;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ISelectedObjectsManager
    {
        public bool allowSelection { get;  set; }
        public ObjectWrapperDTO wrapperDTO { get; }
        public void ClearSelection();
        public GameObject AddObjectToSelection(GameObject selectedObject);
        public GameObject RemoveObjectFromSelection(GameObject selectedObject);
    }
}
