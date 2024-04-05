
namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface ISendStartupMessage
    {
        public string version_build { get; }
        public void SendMessage();
    }
}
