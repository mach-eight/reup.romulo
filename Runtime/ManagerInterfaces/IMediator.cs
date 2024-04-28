using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IMediator
    {
        public void Notify(ReupEvent eventName);
        public void Notify<T>(ReupEvent eventName, T payload);
    }
}
