using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IInsertObjectsManager
    {
        public void InsertObject(InsertObjectMessagePayload insertObjectMessagePayload);
    }
}
