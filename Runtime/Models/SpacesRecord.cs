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
        string currentIdRequest = null;
        string currentRequestSpaceId = null;

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
            GameObject[] spaces;
            try
            {
                spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            }
            catch
            {
                spaces = new GameObject[] { };
            }
            _jumpPoints.Clear();
            foreach (GameObject room in spaces)
            {
                SpaceJumpPoint roomSelector = room.GetComponent<SpaceJumpPoint>();
                _jumpPoints.Add(roomSelector);
            }
        }

        public void GoToSpace(JObject jumpInfo)
        {
            string requestId = jumpInfo["requestId"].ToString();
            string spaceJumpPointId = jumpInfo["spaceId"].ToString();
            CheckIfThereIsACurrentRequest(jumpInfo);
            SpaceJumpPoint spaceSelector = jumpPoints.Find(space => space.id == spaceJumpPointId) as SpaceJumpPoint;
            if (spaceSelector == null)
            {
                NotifyFailure(ReupEvent.spaceJumpPointNotFound, jumpInfo);
                return;
            }
            var groundHit = GetGroundHit(spaceSelector);
            if (groundHit == null)
            {
                NotifyFailure(ReupEvent.spaceJumpPointWithNoGround, jumpInfo);
                return;
            }
            Vector3 positionToJumpTo = GetPositionToJumpTo(spaceSelector, groundHit.Value);
            UnityEvent reachPointEvent = CreateEndMovementEvent(spaceJumpPointId, requestId);
            SlideToSpacePoint(positionToJumpTo, reachPointEvent);
        }

        private void CheckIfThereIsACurrentRequest(JObject jumpInfo)
        {
            if (currentIdRequest != null)
            {
                JObject previousJumpInfo = new JObject
                {
                    { "spaceId", currentRequestSpaceId },
                    { "requestId", currentIdRequest },
                };
                mediator.Notify(ReupEvent.slideToSpacePointRequestInterrupted, previousJumpInfo);
            }
            currentIdRequest = jumpInfo["requestId"].ToString();
            currentRequestSpaceId = jumpInfo["spaceId"].ToString();
        }

        Vector3 GetPositionToJumpTo(SpaceJumpPoint spaceSelector, RaycastHit groundHit)
        {
            Vector3 spaceSelectorPosition = spaceSelector.transform.position;
            float desiredHeight = groundHit.point.y + _characterHeightReseter.CharacterHeight;
            return new Vector3(spaceSelectorPosition.x, desiredHeight, spaceSelectorPosition.z);
        }

        void SlideToSpacePoint(Vector3 targetPosition, UnityEvent endMovementEvent)
        {
            _characterHeightReseter.ResetCharacterHeight();
            _characterPositionManager.MakeKinematic();
            _characterPositionManager.allowWalking = false;
            _characterPositionManager.allowSetHeight = false;
            _characterPositionManager.SlideToTarget(targetPosition, endMovementEvent);
        }

        UnityEvent CreateEndMovementEvent(string spaceJumpPointId, string requestId)
        {
            var endMovementEvent = new UnityEvent();
            endMovementEvent.AddListener(EndMovementHandler);
            endMovementEvent.AddListener(() => NotifySuccess(spaceJumpPointId, requestId));
            return endMovementEvent;
        }

        void NotifyFailure(ReupEvent failureEvent, JObject jumpInfo)
        {
            mediator.Notify(failureEvent, jumpInfo);
        }

        void NotifySuccess(string spaceJumpPointId, string requestId)
        {
            JObject payload = new JObject
            {
                { "spaceId", spaceJumpPointId },
                { "requestId", requestId },
            };
            mediator.Notify(ReupEvent.spaceJumpPointReached, payload);
            currentIdRequest = null;
            currentRequestSpaceId = null;
        }

        private void EndMovementHandler()
        {
            _characterPositionManager.UndoKinematic();
            _characterPositionManager.allowWalking = true;
            _characterPositionManager.allowSetHeight = true;
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
