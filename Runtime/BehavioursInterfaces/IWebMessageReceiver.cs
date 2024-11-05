using System.Threading.Tasks;

namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface IWebMessageReceiver
    {
        public void ReceiveWebMessage(string serializedWebMessage);
    }
}
