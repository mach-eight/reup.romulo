using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class SetUpBuildingDependencyInjector : MonoBehaviour
    {
        private void Awake()
        {
            SetupBuilding setUpBuilding = GetComponent<SetupBuilding>();
            setUpBuilding.colliderAdder = new ColliderAdder();
            setUpBuilding.idAssignerController = new IdController();
        }
    }
}
