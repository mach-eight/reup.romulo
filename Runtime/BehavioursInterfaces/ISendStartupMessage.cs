
namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface ISendStartupMessage
    {
        public string buildVersion { get; }
        public void SendMessage();
    }
}
