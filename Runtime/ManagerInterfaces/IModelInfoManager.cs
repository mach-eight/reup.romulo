using Newtonsoft.Json.Linq;
using ReupVirtualTwin.dataModels;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IModelInfoManager
    {
        public WebMessage<JObject> ObtainModelInfoMessage();
        public WebMessage<UpdateBuildingMessage> ObtainUpdateBuildingMessage();
        public void InsertObjectToBuilding(GameObject obj);
    }
}
