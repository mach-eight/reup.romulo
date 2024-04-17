using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IChangeMaterialController
    {
        public void ChangeObjectMaterial(ChangeMaterialMessagePayload message);
    }
}
