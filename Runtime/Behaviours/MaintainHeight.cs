using UnityEngine;

using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class MaintainHeight : MonoBehaviour, IMaintainHeight
    {
        ICharacterPositionManager _characterPositionManager;
        public ICharacterPositionManager characterPositionManager { set => _characterPositionManager = value; }
        private float _maxStepHeight;
        public float maxStepHeight { set => _maxStepHeight = value; }

        private static float CHARACTER_HEIGHT;
        public float characterHeight { set =>  CHARACTER_HEIGHT = value; }

        private ISensor _sensor;
        public ISensor sensor {  set =>  _sensor = value; }

        private void Start()
        {
            _characterPositionManager.maxStepHeight = _maxStepHeight;
        }

        void Update()
        {
            var hit = _sensor.Sense();
            if (hit != null )
            {
                KeepCharacterHeightFromGround((RaycastHit)hit);
            }
        }

        void KeepCharacterHeightFromGround(RaycastHit groundHit)
        {
            this.groundHit = groundHit.point;
            float newHeight = GetDesiredHeightInGround(groundHit.point.y);
            _characterPositionManager.KeepHeight(newHeight);
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
