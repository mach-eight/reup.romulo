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
    public class SetupBuilding : MonoBehaviour, IOnBuildingSetup
    {

        GameObject building;

        private bool buildingSetup = false;

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
    }
}
