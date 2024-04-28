using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ITransformObjectsManager
    {
        public bool active { get; }
        public ObjectWrapperDTO wrapper { set; }
        public void ActivateTransformMode(ObjectWrapperDTO wrapper, TransformMode mode);
        public void DeactivateTransformMode();
    }
}
