using UnityEngine;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class InitialSpawn : MonoBehaviour, IInitialSpawn
    {
        [SerializeField]
        ICharacterPositionManager characterPositionManager;
        IOnBuildingSetup _setUpBuilding;
        public IOnBuildingSetup setUpBuilding { set => _setUpBuilding = value; }
        private IPointSensor _sensor;
        public IPointSensor sensor { set => _sensor = value; }
        public void Spawn()
        {
            _setUpBuilding.onBuildingSetUp += this.MoveToInitialHeightCallback;
        }

        [Inject]
        public void Init(ICharacterPositionManager characterPositionManager)
        {
            this.characterPositionManager = characterPositionManager;
        }

        private void MoveToInitialHeightCallback()
        {
            RaycastHit? hit = _sensor.Sense();
            if (hit != null)
            {
                float initialGroundHeight = ((RaycastHit)hit).point.y;
                float desiredInitialHeight = MaintainHeight.GetDesiredHeightInGround(initialGroundHeight);
                Vector3 currentPosition = characterPositionManager.characterPosition;
                Vector3 desiredInitialPosition = new Vector3(currentPosition.x, desiredInitialHeight, currentPosition.z);
                characterPositionManager.characterPosition = desiredInitialPosition;
            }
            _setUpBuilding.onBuildingSetUp -= this.MoveToInitialHeightCallback;
        }
    }
}
