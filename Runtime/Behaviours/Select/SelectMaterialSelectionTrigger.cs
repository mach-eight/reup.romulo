using UnityEngine;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.behaviours
{
    public class SelectMaterialSelectionTrigger : SelectObject
    {
        public override void HandleObject(GameObject triggerObject)
        {
            var trigger = triggerObject.GetComponent<IMaterialSelectionTrigger>();
            trigger.CreateContainer();
        }
    }
}
