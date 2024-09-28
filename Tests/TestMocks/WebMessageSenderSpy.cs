using System.Collections.Generic;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwinTests.mocks
{
    public class WebMessageSenderSpy : IWebMessagesSender
    {
        public List<object> sentMessages = new List<object>();

        public void SendWebMessage<T>(WebMessage<T> webMessage)
        {
            sentMessages.Add(webMessage);
        }
    }

}