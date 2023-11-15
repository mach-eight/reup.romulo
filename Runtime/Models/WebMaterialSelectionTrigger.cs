using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.models
{
    public class WebMaterialSelectionTrigger : MonoBehaviour, IWebMaterialSelectionTrigger
    {
        IWebMaterialContainerHandler _containerHandler;
        IUniqueIdentifer triggerIdentifier;

        private void Start()
        {
            _containerHandler = ObjectFinder.FindextensionsTriggers().GetComponent<IWebMaterialContainerHandler>();
        }

        public GameObject CreateContainer()
        {
            return _containerHandler.CreateContainer(this);
        }

        public WebMessage GetWebContainerMessage()
        {
            triggerIdentifier = gameObject.GetComponent<IUniqueIdentifer>();
            var message = new WebMessage
            {
                operation = WebOperationsEnum.showMaterialsOptions,
                body = triggerIdentifier.getId()
            };
            return message;
        }
    }
}
