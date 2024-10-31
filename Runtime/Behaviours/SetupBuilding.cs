using UnityEngine;
using System;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.enums;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class SetupBuilding : MonoBehaviour, ISetUpBuilding, IOnBuildingSetup
    {

        public GameObject building;

        private bool buildingSetup = false;
        private ITagSystemController _tagSystemController;
        public ITagSystemController tagSystemController { get => _tagSystemController; set => _tagSystemController = value; }

        private IObjectInfoController _objectInfoController;
        public IObjectInfoController objectInfoController { get => _objectInfoController; set => _objectInfoController = value; }


        event Action _onBuildingSetUp;
        public event Action onBuildingSetUp
        {
            add
            {
                if (buildingSetup)
                {
                    value();
                }
                else _onBuildingSetUp += value;
            }
            remove { _onBuildingSetUp -= value; }
        }

        private IColliderAdder _colliderAdder;
        public IColliderAdder colliderAdder { set => _colliderAdder = value; }
        private IIdAssignerController _idAssignerController;
        public IIdAssignerController idAssignerController { get => _idAssignerController; set => _idAssignerController = value; }

        [Inject]
        public void Init(
            [Inject(Id = "building")] GameObject building)
        {
            this.building = building;
        }

        void Start()
        {
            if (building != null)
            {
                _colliderAdder.AddCollidersToTree(building);
                GameObjectUtils.ApplyLayerToObjectTree(building, RomuloLayerIds.buildingLayerId);
            }
            else
            {
                Debug.LogError("Building object not set up");
            }
            _onBuildingSetUp?.Invoke();
            buildingSetup = true;
        }

        public void AssignIdsAndObjectInfoToBuilding()
        {
            _idAssignerController.AssignIdsToTree(building);
            _objectInfoController.AssignObjectInfoToTree(building);
        }
        public void RemoveIdsAndObjectInfoFromBuilding()
        {
            _idAssignerController.RemoveIdsFromTree(building);
            _objectInfoController.RemoveObjectInfoFromTree(building);
        }
        public void ResetIdsOfBuilding()
        {
            RemoveIdsAndObjectInfoFromBuilding();
            AssignIdsAndObjectInfoToBuilding();
        }

        public void AddTagSystemToBuildingObjects()
        {
            _tagSystemController.AssignTagSystemToTree(building);
        }
    }
}
