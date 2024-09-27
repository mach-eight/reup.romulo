using UnityEngine;
using UnityEngine.Events;
using System;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.behaviours
{
    public class GoToSpace : MonoBehaviour
    {
        ICharacterPositionManager _characterPositionManager;
        ICharacterHeightReseter _characterHeightReseter;

        private void Start()
        {
            _characterPositionManager = ObjectFinder.FindCharacter().GetComponent<ICharacterPositionManager>();
            _characterHeightReseter = ObjectFinder.FindHeighMediator().GetComponent<ICharacterHeightReseter>();
        }

        public void Go(SpaceJumpPoint spaceSelector)
        {
            _characterPositionManager.MakeKinematic();
            var spaceSelectorPosition = spaceSelector.transform.position;
            _characterHeightReseter.ResetCharacterHeight();
            spaceSelectorPosition.y = GetDesiredHeight(spaceSelector);
            var endMovementEvent = new UnityEvent();
            endMovementEvent.AddListener(EndMovementHandler);
            _characterPositionManager.allowWalking = false;
            _characterPositionManager.allowSetHeight = false;
            _characterPositionManager.SlideToTarget(spaceSelectorPosition, endMovementEvent);
        }

        private void EndMovementHandler()
        {
            _characterPositionManager.UndoKinematic();
            _characterPositionManager.allowWalking = true;
            _characterPositionManager.allowSetHeight = true;
        }

        private float GetDesiredHeight(SpaceJumpPoint spaceSelector)
        {
            var groundHit = GetGroundHit(spaceSelector);
            if (groundHit == null)
            {
                throw new Exception("No Ground below Space selector");
            }
            return ((RaycastHit)groundHit).point.y + _characterHeightReseter.CharacterHeight;
        }

        private RaycastHit? GetGroundHit(SpaceJumpPoint spaceSelector)
        {
            RaycastHit hit;
            var ray = GetDownRayFromSpaceSelector(spaceSelector);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit;
            }
            return null;
        }
        private Ray GetDownRayFromSpaceSelector(SpaceJumpPoint spaceSelector)
        {
            return new Ray(spaceSelector.transform.position, Vector3.down);
        }
    }
}
