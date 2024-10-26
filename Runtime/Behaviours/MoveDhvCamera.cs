using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class MoveDhvCamera : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapperTransform;
        [SerializeField] public GameObject gesturesManagerGameObject;
        [SerializeField] public float limitDistanceFromBuildingInMeters = 35;
        [SerializeField] public float KeyboardMoveCameraSpeedMetersPerSecond = 40;

        public Vector3 hitPoint;
        public Vector3 originalCameraPosition;
        public Vector3 originalWrapperPosition;

        InputProvider _inputProvider;
        IDragManager dragManager;
        IGesturesManager gesturesManager;
        GameObject building;
        private Vector3 buildingCenter;
        private float baseFieldOfView = 60;
        IZoomPositionRotationDHVController zoomPositionRotationDHVController;

        private void OnEnable()
        {
            _inputProvider.holdStarted += OnHoldStarted;
        }
        private void OnDisable()
        {
            _inputProvider.holdStarted -= OnHoldStarted;
        }

        void OnHoldStarted(InputAction.CallbackContext ctx)
        {
            UpdateOriginalPositions();
        }

        [Inject]
        public void Init(
            IDragManager dragManager,
            InputProvider inputProvider,
            [Inject(Id = "building")] GameObject building,
            IZoomPositionRotationDHVController zoomPositionRotationDHVController,
            IGesturesManager gesturesManager)
        {
            _inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.gesturesManager = gesturesManager;
            this.building = building;
            this.zoomPositionRotationDHVController = zoomPositionRotationDHVController;
        }

        void Start()
        {
            buildingCenter = BoundariesUtils.CalculateCenter(building);
        }

        void Update()
        {
            KeyboardUpdatePosition();
            PointerUpdatePosition();
        }

        void KeyboardUpdatePosition()
        {
            Vector2 inputValue = _inputProvider.KeyboardMoveDhvCamera().normalized;
            zoomPositionRotationDHVController.moveInDirection(inputValue, KeyboardMoveCameraSpeedMetersPerSecond);
        }

        void PointerUpdatePosition()
        {
            if (gesturesManager.gestureInProgress)
            {
                UpdateOriginalPositions();
                return;
            }
            if (!dragManager.dragging)
            {
                return;
            }
            Ray cameraRay = RayUtils.GetRayFromCameraToScreenPoint(Camera.main, _inputProvider.PointerInput());
            Ray invertedRay = new Ray(hitPoint, -cameraRay.direction);
            Vector3 newCameraPosition = RayUtils.ProjectRayToHeight(invertedRay, originalCameraPosition.y);
            Vector3 newWrapperPosition = originalWrapperPosition + (newCameraPosition - originalCameraPosition);
            PerformMovement(newWrapperPosition);
        }

        private void UpdateOriginalPositions()
        {
            Ray hitRay = Camera.main.ScreenPointToRay(_inputProvider.PointerInput());
            hitPoint = RayUtils.GetHitPoint(hitRay);
            originalCameraPosition = hitRay.origin;
            originalWrapperPosition = dollhouseViewWrapperTransform.position;
        }

        private void PerformMovement(Vector3 nextPosition)
        {
            if (!isNextPositionInsideBoundaries(nextPosition))
            {
                return;
            }
            dollhouseViewWrapperTransform.position = nextPosition;
        }

        private bool isNextPositionInsideBoundaries(Vector3 positionToCheck)
        {
            Vector3 offsetFromCenter = positionToCheck - buildingCenter;

            bool withinXBounds = Mathf.Abs(offsetFromCenter.x) <= limitDistanceFromBuildingInMeters;
            bool withinZBounds = Mathf.Abs(offsetFromCenter.z) <= limitDistanceFromBuildingInMeters;

            return withinXBounds && withinZBounds;
        }

        public float GetKeyboardMoveCameraRelativeSpeed()
        {
            return KeyboardMoveCameraSpeedMetersPerSecond * GetFieldOfViewMultiplier();
        }

        public float GetFieldOfViewMultiplier()
        {
            return Camera.main.fieldOfView / baseFieldOfView;
        }
    }
}
