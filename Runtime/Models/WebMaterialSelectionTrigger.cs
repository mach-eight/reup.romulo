using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
    public class WebMaterialSelectionTrigger : MonoBehaviour, IWebMaterialSelectionTrigger
    {
        IWebMaterialContainerHandler _containerHandler;
        IUniqueIdentifier triggerIdentifier;

        private void Start()
        {
            _containerHandler = ObjectFinder.FindExtensionsTriggers().GetComponent<IWebMaterialContainerHandler>();
        }

        public GameObject CreateContainer()
        {
            return _containerHandler.CreateContainer();
        }

        public WebMessage<string> GetCreateWebContainerMessage()
        {
            triggerIdentifier = gameObject.GetComponent<IUniqueIdentifier>();
            var message = new WebMessage<string>
            {
                type = WebMessageType.showMaterialsOptions,
                payload = triggerIdentifier.getId()
            };
            return message;
        }
    }
}
