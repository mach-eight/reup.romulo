using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IInsertObjectsController
    {
        public void InsertObject(InsertObjectMessagePayload message);
    }
}
