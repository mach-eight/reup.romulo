using System.Threading;
using System.Threading.Tasks;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class PointerRotation : MonoBehaviour
    {
        [SerializeField]
        ICharacterRotationManager characterRotationManager;
        IDragManager dragManager;
        InputProvider inputProvider;
        IGesturesManager gesturesManager;
        Vector3 startDragRayDirection;
        Vector3 startRayDirProjectedToHorizontalPlane;
        Vector3 startDragRayRightDirection;
        bool followingCursor = false;
        CancellationTokenSource cancellationTokenSource;
        readonly float fraction_to_reach_target = 0.5f;

        [Inject]
        public void Init(
            InputProvider inputProvider,
            IDragManager dragManager,
            ICharacterRotationManager characterRotationManager,
            IGesturesManager gesturesManager)
        {
            this.inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.gesturesManager = gesturesManager;
            this.inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.characterRotationManager = characterRotationManager;
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
            if (!dragManager.primaryDragging || gesturesManager.gestureInProgress)
            {
                cancellationTokenSource?.Cancel();
                return;
            }
            if (followingCursor == true)
            {
                return;
            }
            _ = FollowCursor();
        }
        async Task FollowCursor()
        {
            followingCursor = true;
            cancellationTokenSource = new CancellationTokenSource();
            while (followingCursor == true)
            {
                Ray dragRay = Camera.main.ScreenPointToRay(inputProvider.PointerInput());
                float dragRayAngleDifference = Vector3.Angle(startDragRayDirection, dragRay.direction);
                if (dragRayAngleDifference < characterRotationManager.ANGLE_THRESHOLD || cancellationTokenSource.Token.IsCancellationRequested)
                {
                    followingCursor = false;
                    break;
                }
                float horizontalRotationAngle = GetHorizontalRotationAngle(dragRay.direction);
                characterRotationManager.horizontalRotation += horizontalRotationAngle;
                float verticalRotationAngle = GetVerticalRotationAngle(dragRay.direction);
                characterRotationManager.verticalRotation += verticalRotationAngle;
                await Task.Yield();
            }
            followingCursor = false;
        }

        float GetHorizontalRotationAngle(Vector3 currentDragRayDirection)
        {
            Vector3 currentRayProjectedToHorizontal = Vector3.ProjectOnPlane(currentDragRayDirection, Vector3.up);
            float angle = Vector3.SignedAngle(startRayDirProjectedToHorizontalPlane, currentRayProjectedToHorizontal, Vector3.up);
            return -angle * fraction_to_reach_target;
        }
        float GetVerticalRotationAngle(Vector3 currentDragRayDirection)
        {
            Vector3 currentRayProjectedToVertical = Vector3.ProjectOnPlane(currentDragRayDirection, startDragRayRightDirection);
            float angle = Vector3.SignedAngle(startDragRayDirection, currentRayProjectedToVertical, startDragRayRightDirection);
            return -angle * fraction_to_reach_target;
        }
    }
}