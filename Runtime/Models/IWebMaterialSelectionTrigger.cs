using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.models
{
    public interface IWebMaterialSelectionTrigger: IMaterialSelectionTrigger
    {
        public WebMessage<string> GetCreateWebContainerMessage();
    }
}
