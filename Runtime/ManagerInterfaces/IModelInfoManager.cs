using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IModelInfoManager
    {
        public WebMessage<ModelInfoMessage> ObtainModelInfoMessage();
    }
}
