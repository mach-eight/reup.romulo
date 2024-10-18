using UnityEngine;

using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.behaviourInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class MaintainHeight : MonoBehaviour, IMaintainHeight
    {
        ICharacterPositionManager characterPositionManager;
        private float _maxStepHeight;
        public float maxStepHeight { set => _maxStepHeight = value; }

        private static float CHARACTER_HEIGHT;
        public float characterHeight { set => CHARACTER_HEIGHT = value; }

        private IPointSensor _sensor;
        public IPointSensor sensor { set => _sensor = value; }

        [Inject]
        public void Init(ICharacterPositionManager characterPositionManager)
        {
            this.characterPositionManager = characterPositionManager;
        }

        private void Start()
        {
            characterPositionManager.maxStepHeight = _maxStepHeight;
        }

        void Update()
        {
            var hit = _sensor.Sense();
            if (hit != null)
            {
                KeepCharacterHeightFromGround((RaycastHit)hit);
            }
        }

        void KeepCharacterHeightFromGround(RaycastHit groundHit)
        {
            this.groundHit = groundHit.point;
            float newHeight = GetDesiredHeightInGround(groundHit.point.y);
            characterPositionManager.KeepHeight(newHeight);
        }

        Vector3 groundHit = Vector3.zero;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundHit, 0.05f);
        }

        public static float GetDesiredHeightInGround(float groundHeight)
        {
            return groundHeight + CHARACTER_HEIGHT;
        }
    }
}
