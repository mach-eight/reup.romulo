using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class PointerRotation : MonoBehaviour
    {
        public float sensitivity = 0.4f;

        [SerializeField]
        CharacterRotationManager _characterRotationManager;
        IDragManager dragManager;
        InputProvider inputProvider;
        IGesturesManager gesturesManager;
        Vector3 startDragRayDirection;
        Vector3 startRayDirProjectedToHorizontalPlane;
        Vector3 startDragRayRightDirection;

        [Inject]
        public void Init(InputProvider inputProvider, IDragManager dragManager, IGesturesManager gesturesManager)
        {
            this.inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.gesturesManager = gesturesManager;
        }

        private void OnEnable()
        {
            inputProvider.holdStarted += OnHoldStarted;
        }
        private void OnDisable()
        {
            inputProvider.holdStarted -= OnHoldStarted;
        }

        void OnHoldStarted(InputAction.CallbackContext ctx)
        {
            Ray startDragRay = Camera.main.ScreenPointToRay(inputProvider.PointerInput());
            startDragRayDirection = startDragRay.direction;
            startRayDirProjectedToHorizontalPlane = Vector3.Normalize(Vector3.ProjectOnPlane(startDragRayDirection, Vector3.up));
            float hemisphereFactor = startDragRayDirection.y >= 0 ? -1 : 1;
            startDragRayRightDirection = Vector3.Normalize(Vector3.Cross(startRayDirProjectedToHorizontalPlane, startDragRayDirection) * hemisphereFactor);
            if (Vector3.Magnitude(startDragRayRightDirection) == 0)
            {
                startDragRayRightDirection = Vector3.Normalize(Vector3.Cross(Vector3.up, startRayDirProjectedToHorizontalPlane));
            }
        }

        void Update()
        {
            if (!dragManager.dragging || gesturesManager.gestureInProgress)
            {
                return;
            }
            Ray dragRay = Camera.main.ScreenPointToRay(inputProvider.PointerInput());
            float horizontalRotationAngle = GetHorizontalRotationAngle(dragRay.direction);
            _characterRotationManager.horizontalRotation += horizontalRotationAngle;
            float verticalRotationAngle = GetVerticalRotationAngle(dragRay.direction);
            _characterRotationManager.verticalRotation += verticalRotationAngle;
        }

        float GetHorizontalRotationAngle(Vector3 currentDragRayDirection)
        {
            Vector3 currentRayProjectedToHorizontal = Vector3.ProjectOnPlane(currentDragRayDirection, Vector3.up);
            float angle = Vector3.SignedAngle(startRayDirProjectedToHorizontalPlane, currentRayProjectedToHorizontal, Vector3.up);
            return -angle;
        }
        float GetVerticalRotationAngle(Vector3 currentDragRayDirection)
        {
            Vector3 currentRayProjectedToVertical = Vector3.ProjectOnPlane(currentDragRayDirection, startDragRayRightDirection);
            float angle = Vector3.SignedAngle(startDragRayDirection, currentRayProjectedToVertical, startDragRayRightDirection);
            return -angle;
        }
    }
}