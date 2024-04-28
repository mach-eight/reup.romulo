using UnityEngine;
using ReupVirtualTwin.models;
using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IWebMessagesSender))]
    public class SelectWebMaterialSelectionTrigger : SelectObject
    {
        IWebMessagesSender _webMessagesSender;

        protected override void Start()
        {
            base.Start();
            _webMessagesSender = GetComponent<IWebMessagesSender>();
        }

        public override void HandleObject(GameObject triggerObject)
        {
            var trigger = triggerObject.GetComponent<IWebMaterialSelectionTrigger>();
            var container = trigger.CreateContainer();
            if (container != null)
            {
                SendCreateWebContainerMessage(trigger);
            }
        }

        void SendCreateWebContainerMessage(IWebMaterialSelectionTrigger trigger)
        {
            var createWebMaterialsContainerMessage = trigger.GetCreateWebContainerMessage();
            _webMessagesSender.SendWebMessage(createWebMaterialsContainerMessage);
        }
    }
}
