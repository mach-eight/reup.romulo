using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class MoveDhvCamera : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapperTransform;
        [SerializeField] public GameObject dragManagerGameObject;
        [SerializeField] public GameObject gesturesManagerGameObject;
        [SerializeField] public float limitDistanceFromBuildingInMeters = 35;
        [SerializeField] public float KeyboardMoveCameraSpeedMetersPerSecond = 40;
        [SerializeField] public float PointerMoveCameraDistanceInMetersSquareViewport = 40;

        InputProvider _inputProvider;
        float distancePerPixel;
        DragManager dragManager;
        IGesturesManager gesturesManager;
        GameObject building;
        private Vector3 buildingCenter;
        private float baseFieldOfView = 60;

        void Awake()
        {
            _inputProvider = new InputProvider();
            int pixelsInSquareViewport = ViewportUtils.MinViewportDimension(Camera.main);
            distancePerPixel = PointerMoveCameraDistanceInMetersSquareViewport / pixelsInSquareViewport;
            dragManager = dragManagerGameObject.GetComponent<DragManager>();
            gesturesManager = gesturesManagerGameObject.GetComponent<IGesturesManager>();
            building = ObjectFinder.FindSetupBuilding().GetComponent<SetupBuilding>().building;
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

            float fovMultiplier = GetFieldOfViewMultiplier();

            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * inputValue.x + cameraForward * inputValue.y;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);

            float movementDistance = KeyboardMoveCameraSpeedMetersPerSecond * fovMultiplier * Time.deltaTime;
            Vector3 nextPosition = dollhouseViewWrapperTransform.position + (normalizedDirection * movementDistance);

            PerformMovement(nextPosition);
        }

        void PointerUpdatePosition()
        {
            if (!dragManager.dragging || gesturesManager.gestureInProgress)
            {
               return;
            }

            Vector2 inputValue = _inputProvider.PointerMoveDhvCamera();
            if (inputValue == Vector2.zero)
            {
                return;
            }

            float fovMultiplier = GetFieldOfViewMultiplier();

            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            float sideMovement = inputValue.x * distancePerPixel * fovMultiplier;
            float forwardMovement = inputValue.y * distancePerPixel * fovMultiplier;
            Vector3 movement = cameraRight * sideMovement + cameraForward * forwardMovement;
            Vector3 nextPosition = dollhouseViewWrapperTransform.position + movement;

            PerformMovement(nextPosition);
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

        private float GetFieldOfViewMultiplier()
        {
            return Camera.main.fieldOfView / baseFieldOfView;
        }
    }
}
