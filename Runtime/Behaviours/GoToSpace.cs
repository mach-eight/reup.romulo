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
        SpaceJumpPoint spaceSelector;

        private void Start()
        {
            _characterPositionManager = ObjectFinder.FindCharacter().GetComponent<ICharacterPositionManager>();
            spaceSelector = GetComponent<SpaceButtonInstance>().spaceSelector;
            _characterHeightReseter = ObjectFinder.FindHeighMediator().GetComponent<ICharacterHeightReseter>();
        }

        public void Go()
        {
            _characterPositionManager.MakeKinematic();
            var spaceSelectorPosition = spaceSelector.transform.position;
            _characterHeightReseter.ResetCharacterHeight();
            spaceSelectorPosition.y = GetDesiredHeight();
            var endMovementEvent = new UnityEvent();
            endMovementEvent.AddListener(EndMovementHandler);
            _characterPositionManager.allowWalking = false;
            _characterPositionManager.allowSetHeight = false;
            _characterPositionManager.SlideToTarget(spaceSelectorPosition,endMovementEvent);
        }

        private void EndMovementHandler()
        {
            _characterPositionManager.UndoKinematic();
            _characterPositionManager.allowWalking = true;
            _characterPositionManager.allowSetHeight = true;
        }

        private float GetDesiredHeight()
        {
            var groundHit = GetGroundHit();
            if (groundHit == null)
            {
                throw new Exception("No Ground below Space selector");
            }
            return ((RaycastHit)groundHit).point.y + _characterHeightReseter.CharacterHeight;
        }

        private RaycastHit? GetGroundHit()
        {
            RaycastHit hit;
            var ray = GetDownRayFromSpaceSelector();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit;
            }
            return null;
        }
        private Ray GetDownRayFromSpaceSelector()
        {
            return new Ray(spaceSelector.transform.position, Vector3.down);
        }
    }
}
