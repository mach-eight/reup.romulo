using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.enums;
using System;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.helpers;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.models
{

    public class SpacesRecord : MonoBehaviour, ISpacesRecord
    {
        [SerializeField] LayerMask ignoreLayerMask;
        ICharacterPositionManager _characterPositionManager;
        ICharacterHeightReseter _characterHeightReseter;
        List<ISpaceJumpPoint> _jumpPoints;
        public IMediator mediator { get; set; }
        public List<ISpaceJumpPoint> jumpPoints
        {
            get
            {
                if (_jumpPoints == null)
                {
                    UpdateSpaces();
                }
                return _jumpPoints;
            }
            set => _jumpPoints = value;
        }

        private void Awake()
        {
            _characterPositionManager = ObjectFinder.FindCharacter().GetComponent<ICharacterPositionManager>();
            _characterHeightReseter = ObjectFinder.FindHeighMediator().GetComponent<ICharacterHeightReseter>();
            mediator = ObjectFinder.FindEditMediator();
        }

        public void UpdateSpaces()
        {
            if (_jumpPoints == null)
            {
                _jumpPoints = new List<ISpaceJumpPoint>() { };
            }
            if (!SpaceTagIsDefined())
            {
                return;
            }
            GameObject[] spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            _jumpPoints.Clear();
            foreach (GameObject room in spaces)
            {
                SpaceJumpPoint roomSelector = room.GetComponent<SpaceJumpPoint>();
                _jumpPoints.Add(roomSelector);
            }
        }

        public bool SpaceTagIsDefined()
        {
            return Array.Exists(
                UnityEditorInternal.InternalEditorUtility.tags,
                element => element == TagsEnum.spaceSelector
            );
        }

        public void GoToSpace(string spaceJumpPointId, string requestId)
        {
            SpaceJumpPoint spaceSelector = jumpPoints.Find(space => space.id == spaceJumpPointId) as SpaceJumpPoint;
            _characterPositionManager.MakeKinematic();
            var spaceSelectorPosition = spaceSelector.transform.position;
            _characterHeightReseter.ResetCharacterHeight();
            spaceSelectorPosition.y = GetDesiredHeight(spaceSelector);
            var endMovementEvent = new UnityEvent();
            endMovementEvent.AddListener(EndMovementHandler);
            endMovementEvent.AddListener(() => mediator.Notify(ReupEvent.spaceJumpPointReached, CreateNotifyPayload(spaceJumpPointId, requestId)));
            _characterPositionManager.allowWalking = false;
            _characterPositionManager.allowSetHeight = false;
            _characterPositionManager.SlideToTarget(spaceSelectorPosition, endMovementEvent);
        }

        private JObject CreateNotifyPayload(string spaceJumpPointId, string requestId)
        {
            return new JObject
            {
                { "spaceId", spaceJumpPointId },
                { "requestId", requestId },
            };
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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayerMask))
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
