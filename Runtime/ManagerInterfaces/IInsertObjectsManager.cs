using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IInsertObjectsManager
    {
        public void InsertObjectFromUrl(InsertObjectMessagePayload insertObjectMessagePayload);
    }
}
