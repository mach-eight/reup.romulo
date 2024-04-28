using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.models;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IWebMessagesSender))]
    public class HideWebMaterials : SelectObject
    {
        IMaterialsContainerHider _webMaterialContainerHider;
        IWebMessagesSender _webMessagesSender;

        protected override void Start()
        {
            base.Start();
            _webMaterialContainerHider = ObjectFinder.FindExtensionsTriggers().GetComponent<IMaterialsContainerHider>();
            _webMessagesSender = GetComponent<IWebMessagesSender>();
        }

        public override void MissObject()
        {
            SendHideWebContainerMessage();
            HideContainer();
        }
        void SendHideWebContainerMessage()
        {

            var message = new WebMessage<string>
            {
                type = WebMessageType.hideMaterialsOptions
            };
            _webMessagesSender.SendWebMessage(message);
        }
        void HideContainer()
        {
            _webMaterialContainerHider.HideContainer();
        }
    }
}
