using UnityEngine;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class DetectCollision : MonoBehaviour
    {
        ICharacterPositionManager _positionManager;
        float SMALL_JUMP_FORCE_AT_COLLISION = 0.01f;

        private void Start()
        {
            _positionManager = ObjectFinder.FindCharacter().GetComponent<ICharacterPositionManager>();
        }

        private void OnCollisionEnter()
        {
            _positionManager.allowSetHeight = false;
            _positionManager.allowWalking = false;
            _positionManager.StopRigidBody();
        }
        private void OnCollisionStay(Collision collision)
        {
            var bounceDirection = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                bounceDirection += contact.normal;
            }

            bounceDirection.y = SMALL_JUMP_FORCE_AT_COLLISION;
            _positionManager.MoveDistanceInDirection(0.02f, bounceDirection);
            _positionManager.ApplyForceInDirection(bounceDirection);
        }

        private void OnCollisionExit()
        {
            _positionManager.allowSetHeight = true;
            _positionManager.allowWalking = true;
        }
    }
}
