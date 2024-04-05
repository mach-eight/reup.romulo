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

        string _version_build = "write a date format here in next release";
        private void Start()
        {
            _webMessagesSender = gameObject.GetComponent<IWebMessagesSender>();
            SendMessage();
        }
        public void SendMessage()
        {
            WebMessage<string> message = new()
            {
                type = WebMessageType.statusLoad,
                payload = version_build,
            };
            _webMessagesSender.SendWebMessage(message);
        }
    }
}
