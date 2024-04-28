using UnityEngine;
using UnityEngine.Events;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ICharacterPositionManager
    {
        public bool allowWalking { set; get; }
        public bool allowSetHeight { set; get; }
        public bool allowMovingUp { set; get; }
        public float maxStepHeight { set; }
        public Vector3 characterPosition { get; set; }
        public void StopRigidBody();
        public void MoveDistanceInDirection(float distance, Vector3 direction);
        public void ApplyForceInDirection(Vector3 direction);
        public void WalkToTarget(Vector3 target);
        public void KeepHeight(float height);
        public void SlideToTarget(Vector3 target, UnityEvent endEvent);
        public void SlideToTarget(Vector3 target);
        public void MakeKinematic();
        public void UndoKinematic();
        public void StopWalking();
        public void MoveInDirection(Vector3 direction, float speedInMetersPerSecond);
    }
}
