using System.Collections.Generic;
using System.Linq;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class BuildingVisibilityController : IBuildingVisibilityController
    {
        readonly IObjectRegistry objectRegistry;
        readonly GameObject buildingObject;
        public BuildingVisibilityController(IObjectRegistry objectRegistry, GameObject buildingObject)
        {
            this.objectRegistry = objectRegistry;
            this.buildingObject = buildingObject;
        }
        public TaskResult SetObjectsVisibility(string[] objectsIds, bool show)
        {
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(objectsIds);
            string[] missingObjectsIds = objectsIds.Where((id, index) => objects[index] == null).ToArray();

            if (missingObjectsIds.Length > 0)
            {
                string missingIds = string.Join(", ", missingObjectsIds);
                return TaskResult.Failure($"Hide/Show objects failed: The following object Ids were not found: {missingIds}");
            }

            foreach (GameObject obj in objects)
            {
                obj.SetActive(show);
            }

            return TaskResult.Success();
        }

        public TaskResult ShowAllObjects()
        {
            SetActiveRecursively(buildingObject, true);
            return TaskResult.Success();
        }

        public TaskResult HideAllObjects()
        {
            SetActiveRecursively(buildingObject, false);
            return TaskResult.Success();
        }

        private void SetActiveRecursively(GameObject obj, bool isActive)
        {
            obj.SetActive(isActive);
            foreach (Transform child in obj.transform)
            {
                SetActiveRecursively(child.gameObject, isActive);
            }
        }
    }
}