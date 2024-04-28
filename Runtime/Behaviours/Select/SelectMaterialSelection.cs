using ReupVirtualTwin.helpers;
using ReupVirtualTwin.models;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class SelectMaterialSelection : SelectObject
    {
        IMaterialContainerHandler _materialContainerHandler;

        protected override void Start()
        {
            base.Start();
            _materialContainerHandler = ObjectFinder.FindExtensionsTriggers().GetComponent<IMaterialContainerHandler>();
        }
        public override void HandleObject(GameObject materialSelectionObject)
        {
            var material = materialSelectionObject.GetComponent<Renderer>().material;
            _materialContainerHandler.SetNewMaterial(material);
        }
        public override void MissObject()
        {
            if (!_dragManager.dragging && !_dragManager.prevDragging)
            {
                _materialContainerHandler.HideContainer();
            }
        }
    }
}
