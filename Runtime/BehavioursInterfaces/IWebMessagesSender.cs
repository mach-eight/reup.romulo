using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface IWebMessagesSender
    {
        public void SendWebMessage<T>(WebMessage<T> webWessage);
    }
}
