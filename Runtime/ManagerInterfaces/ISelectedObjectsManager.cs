using ReupVirtualTwin.dataModels;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ISelectedObjectsManager
    {
        public bool allowEditSelection { get;  set; }
        public ObjectWrapperDTO wrapperDTO { get; }
        public void ClearSelection();
        public GameObject AddObjectToSelection(GameObject selectedObject);
        public GameObject RemoveObjectFromSelectionIfEditSelectionAllowed(GameObject selectedObject);
        public GameObject RemoveObjectFromSelection(GameObject selectedObject);
    }
}
