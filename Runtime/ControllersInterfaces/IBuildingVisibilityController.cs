using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IBuildingVisibilityController
    {
        public TaskResult SetObjectsVisibility(string[] objectsIds, bool show);
        public TaskResult ShowAllObjects();
    }
}