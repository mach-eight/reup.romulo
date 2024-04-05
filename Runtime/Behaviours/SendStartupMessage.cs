using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ReupVirtualTwin.behaviours
{
    public class SendStartupMessage: MonoBehaviour, ISendStartupMessage
    {
        public string version_build { get => _version_build; }

        IWebMessagesSender _webMessagesSender;
        IOnBuildingSetup setUpBuilding;

        string _version_build = "write a date format here in next release";
        private void Start()
        {
            setUpBuilding = ObjectFinder.FindSetupBuilding()?.GetComponent<IOnBuildingSetup>();
            _webMessagesSender = gameObject.GetComponent<IWebMessagesSender>();
            setUpBuilding.onBuildingSetUp += SendMessage;
        }
        public void SendMessage()
        {
            StartupMessage startupMessage = new()
            {
                buildVersion = version_build,
            };
            WebMessage<string> message = new()
            {
                type = WebMessageType.statusLoad,
                payload = version_build,
            };
            _webMessagesSender.SendWebMessage(message);
        }
    }
}
