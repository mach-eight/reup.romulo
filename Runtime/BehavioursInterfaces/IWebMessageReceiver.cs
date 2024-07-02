using System.Threading.Tasks;

namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface IWebMessageReceiver
    {
        public Task ReceiveWebMessage(string serializedWebMessage);
    }
}
