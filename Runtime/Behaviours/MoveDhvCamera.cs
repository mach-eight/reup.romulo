using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managers;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class MoveDhvCamera : MonoBehaviour
    {
        public Transform dollhouseViewWrapperTransform;
        public float limitDistanceFromBuildingInMeters = 35;

        InputProvider _inputProvider;
        public static readonly float KEYBOARD_MOVE_CAMERA_SPEED_METERS_PER_SECOND = 40;
        public static readonly float POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT = 40;

        float distancePerPixel;
        DragManager dragManager;
        GameObject building;
        private Vector3 buildingCenter;

        void Awake()
        {
            _inputProvider = new InputProvider();
            int pixelsInSquareViewport = ViewportUtils.MinViewportDimension(Camera.main);
            distancePerPixel = POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT / pixelsInSquareViewport;
            dragManager = ObjectFinder.FindDragManager().GetComponent<DragManager>();
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
            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * inputValue.x + cameraForward * inputValue.y;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);
            float movementDistance = KEYBOARD_MOVE_CAMERA_SPEED_METERS_PER_SECOND * Time.deltaTime;
            Vector3 nextPosition = dollhouseViewWrapperTransform.position + (normalizedDirection * movementDistance);
            
            PerformMovement(nextPosition);
        }

        void PointerUpdatePosition()
        {
            if (!dragManager.dragging)
            {
               return;
            }
            Vector2 inputValue = _inputProvider.PointerMoveDhvCamera();
            if (inputValue == Vector2.zero)
            {
                return;
            }
            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            float sideMovement = inputValue.x * distancePerPixel;
            float forwardMovement = inputValue.y * distancePerPixel;
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

    }
}
