using System.Runtime.InteropServices;
using ReupVirtualTwin.dataModels;
using UnityEngine;
using ReupVirtualTwin.behaviourInterfaces;
using Newtonsoft.Json;

namespace ReupVirtualTwin.behaviours
{
    public class WebMessagesSender : MonoBehaviour, IWebMessagesSender
    {

        [DllImport("__Internal")]
        private static extern void SendStringToWeb(string msg);

        public void SendWebMessage<T>(WebMessage<T> webMessage)
        {
            string serializedMessage = JsonConvert.SerializeObject(webMessage);
            try
            {
                SendStringToWeb(serializedMessage);
            }
            catch
            {
                Debug.LogWarning($"web message sender failed to send: {serializedMessage}");
            }
        }
    }
}
