using UnityEngine;
using System;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class SetupBuilding : MonoBehaviour , ISetUpBuilding, IOnBuildingSetup, IBuildingGetter
    {
        [SerializeField]
        GameObject _building;
        public GameObject building { get => _building; }

        private bool buildingSetup = false;
        private ITagSystemController _tagSystemController;
        public ITagSystemController tagSystemController { get => _tagSystemController; set => _tagSystemController = value; }

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
        //private 

        void Start()
        {
            if (building != null)
            {
                _colliderAdder.AddCollidersToTree(building);
            }
            else
            {
                Debug.LogError("Building object not set up");
            }
            _onBuildingSetUp?.Invoke();
            buildingSetup = true;
        }

        public void AssignIdsToBuilding()
        {
            _idAssignerController.AssignIdsToTree(building);
            Debug.Log("Ids added to tree");
        }
        public void RemoveIdsOfBuilding()
        {
            _idAssignerController.RemoveIdsFromTree(building);
            Debug.Log("Ids removed from tree");
        }
        public void ResetIdsOfBuilding()
        {
            RemoveIdsOfBuilding();
            AssignIdsToBuilding();
            Debug.Log("Ids reseted from tree");
        }

        public void AddTagSystemToBuildingObjects()
        {
            _tagSystemController.AssignTagSystemToTree(building);
            Debug.Log("tags script added to tree");
        }
    }
}