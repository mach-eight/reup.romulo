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
            IGesturesManager gesturesManager)
        {
            _inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.gesturesManager = gesturesManager;
            this.building = building;
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
            if (inputValue == Vector2.zero)
            {
                return;
            }

            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * inputValue.x + cameraForward * inputValue.y;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);

            float movementDistance = GetKeyboardMoveCameraRelativeSpeed() * Time.deltaTime;
            Vector3 nextPosition = dollhouseViewWrapperTransform.position + (normalizedDirection * movementDistance);

            PerformMovement(nextPosition);
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
