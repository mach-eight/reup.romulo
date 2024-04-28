using UnityEngine;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class InitialSpawn : MonoBehaviour, IInitialSpawn
    {
        [SerializeField]
        ICharacterPositionManager _characterPositionManager;
        public ICharacterPositionManager characterPositionManager { set => _characterPositionManager = value; }
        IOnBuildingSetup _setUpBuilding;
        public IOnBuildingSetup setUpBuilding { set => _setUpBuilding = value; }
        private ISensor _sensor;
        public ISensor sensor { set => _sensor = value; }
        public void Spawn()
        {
            _setUpBuilding.onBuildingSetUp += this.MoveToInitialHeightCallback;
        }

        private void MoveToInitialHeightCallback()
        {
            RaycastHit? hit = _sensor.Sense();
            if (hit != null)
            {
                float initialGroundHeight = ((RaycastHit)hit).point.y;
                float desiredInitialHeight = MaintainHeight.GetDesiredHeightInGround(initialGroundHeight);
                Vector3 currentPosition = _characterPositionManager.characterPosition;
                Vector3 desiredInitialPosition = new Vector3(currentPosition.x, desiredInitialHeight, currentPosition.z);
                _characterPositionManager.characterPosition = desiredInitialPosition;
            }
            _setUpBuilding.onBuildingSetUp -= this.MoveToInitialHeightCallback;
        }
    }
}
