using UnityEngine;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class DetectCollision : MonoBehaviour
    {
        ICharacterPositionManager positionManager;
        float SMALL_JUMP_FORCE_AT_COLLISION = 0.01f;

        [Inject]
        public void Init(ICharacterPositionManager characterPositionManager)
        {
            this.positionManager = characterPositionManager;
        }

        private void OnCollisionEnter()
        {
            positionManager.allowSetHeight = false;
            positionManager.allowWalking = false;
            positionManager.StopRigidBody();
        }
        private void OnCollisionStay(Collision collision)
        {
            var bounceDirection = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                bounceDirection += contact.normal;
            }

            bounceDirection.y = SMALL_JUMP_FORCE_AT_COLLISION;
            positionManager.MoveDistanceInDirection(0.02f, bounceDirection);
            positionManager.ApplyForceInDirection(bounceDirection);
        }

        private void OnCollisionExit()
        {
            positionManager.allowSetHeight = true;
            positionManager.allowWalking = true;
        }
    }
}
