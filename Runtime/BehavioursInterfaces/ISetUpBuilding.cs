using System;

namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface ISetUpBuilding
    {
        public void AssignIdsAndObjectInfoToBuilding();
        public void RemoveIdsAndObjectInfoFromBuilding();
        public void ResetIdsOfBuilding();
        public void AddTagSystemToBuildingObjects();
    }
}