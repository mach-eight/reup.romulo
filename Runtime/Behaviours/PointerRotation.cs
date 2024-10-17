using ReupVirtualTwin.helpers;
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
        Vector3 startCameraForwardDirection;
        // float startHorizontalRotationAngle;
        Vector3 startRayDirProjectedToHorizontalPlane;
        // Vector3 startRayDirProjectedToVerticalPlane;
        Vector3 startDragRayLeftDirection;

        private void Start()
        {
            Debug.Log("empezando");
        }

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
            Debug.DrawRay(startDragRay.origin, startDragRay.direction * 100, Color.green, 1000);
            startDragRayDirection = startDragRay.direction;
            Debug.Log($"48: startDragRayDirection >>>\n{startDragRayDirection}");
            startCameraForwardDirection = Camera.main.transform.forward;
            // startHorizontalRotationAngle = _characterRotationManager.horizontalRotation;
            startRayDirProjectedToHorizontalPlane = Vector3.Normalize(Vector3.ProjectOnPlane(startDragRayDirection, Vector3.up));
            Debug.Log($"52: startRayDirProjectedToHorizontalPlane >>>\n{startRayDirProjectedToHorizontalPlane}");
            float sideFactor = startDragRayDirection.y >= 0 ? 1 : -1;
            Debug.Log($"55: sideFactor >>>\n{sideFactor}");
            startDragRayLeftDirection = Vector3.Normalize(Vector3.Cross(startRayDirProjectedToHorizontalPlane, startDragRayDirection) * sideFactor);
            if (Vector3.Magnitude(startDragRayLeftDirection) == 0)
            {
                startDragRayLeftDirection = Vector3.Normalize(Vector3.Cross(startRayDirProjectedToHorizontalPlane, Vector3.up));
            }
            Debug.Log($"54: startDragRayLeftDirection >>>\n{startDragRayLeftDirection}");
            // startRayDirProjectedToVerticalPlane = Vector3.Normalize(Vector3.ProjectOnPlane(startDragRayDirection, startDragRayLeftDirection));
            // Debug.Log($"56: startRayDirProjectedToVerticalPlane >>>\n{startRayDirProjectedToVerticalPlane}");
        }

        void Update()
        {
            Debug.Log("update pointer rotation");
            if (!dragManager.dragging || gesturesManager.gestureInProgress)
            {
                return;
            }
            Debug.Log($"77: Camera.main.transform.forward >>>\n{Camera.main.transform.forward}");
            Ray dragRay = Camera.main.ScreenPointToRay(inputProvider.PointerInput());
            Debug.DrawRay(dragRay.origin, dragRay.direction * 100, Color.red);
            Debug.Log($"55: dragRay.direction >>>\n{dragRay.direction}");
            float horizontalRotationAngle = GetHorizontalRotationAngle(dragRay.direction);
            Debug.Log($"61: horizontalRotationAngle >>>\n{horizontalRotationAngle}");
            Debug.Log($"77: _characterRotationManager.horizontalRotation >>>\n{_characterRotationManager.horizontalRotation}");
            _characterRotationManager.horizontalRotation += horizontalRotationAngle;
            Debug.Log($"79: _characterRotationManager.horizontalRotation >>>\n{_characterRotationManager.horizontalRotation}");
            float verticalRotationAngle = GetVerticalRotationAngle(dragRay.direction);
            // Debug.Log($"71: verticalRotationAngle >>>\n{verticalRotationAngle}");
            // Debug.Log($"78: _characterRotationManager.verticalRotation >>>\n{_characterRotationManager.verticalRotation}");
            _characterRotationManager.verticalRotation += verticalRotationAngle;
            // Debug.Log($"80: _characterRotationManager.verticalRotation >>>\n{_characterRotationManager.verticalRotation}");

        }

        float GetHorizontalRotationAngle(Vector3 currentDragRayDirection)
        {
            Vector3 currentRayProjectedToHorizontal = Vector3.ProjectOnPlane(currentDragRayDirection, Vector3.up);
            Debug.Log($"74: startRayDirectionProjected >>>\n{startRayDirProjectedToHorizontalPlane}");
            Debug.Log($"72: currentRayDirectionProjected >>>\n{currentRayProjectedToHorizontal}");
            float angle = Vector3.SignedAngle(startRayDirProjectedToHorizontalPlane, currentRayProjectedToHorizontal, Vector3.up);
            Debug.Log($"76: angle >>>\n{angle}");
            return -angle;
        }

        float GetVerticalRotationAngle(Vector3 currentDragRayDirection)
        {
            Vector3 currentRayProjectedToVertical = Vector3.ProjectOnPlane(currentDragRayDirection, startDragRayLeftDirection);
            // Debug.Log($"93: startRayDirProjectedToVerticalPlane >>>\n{startRayDirProjectedToVerticalPlane}");
            // Debug.Log($"89: currentRayProjectedToVertical >>>\n{currentRayProjectedToVertical}");
            float angle = Vector3.SignedAngle(startDragRayDirection, currentRayProjectedToVertical, startDragRayLeftDirection);
            return angle;
        }

    }
}