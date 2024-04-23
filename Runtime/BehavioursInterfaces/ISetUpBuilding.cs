using System;

namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface ISetUpBuilding
    {
        public void AssignIdsToBuilding();
        public void RemoveIdsOfBuilding();
        public void ResetIdsOfBuilding();
        public void AddTagSystemToBuildingObjects();
    }
}