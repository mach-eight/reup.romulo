using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.helpers;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.controllers
{
    public class ZoomPositionRotationDHVController : IZoomPositionRotationDHVController
    {
        public Vector2 focusScreenPoint { set => UpdateFocusRay(value); }
        Ray focusRay;
        public Vector2 startingFocusScreenPoint { set => SetStartingFocusRay(value); }
        Ray startingFocusRay;
        Transform dollhouseViewWrapperTransform;
        float baseFieldOfView = 60; // todo: this is a magic number, it should be obtained from the camera in initialization
        Vector3 buildingCenter;
        public float limitDistanceFromBuildingInMeters = 35;

        Vector3 hitPoint;
        Vector3 originalCameraPosition;
        Vector3 originalWrapperPosition;

        [SerializeField] public float KeyboardMoveCameraSpeedMetersPerSecond = 40; // todo inject this from the Reup prefab
        public float keyboardMoveCameraRelativeSpeed { get => KeyboardMoveCameraSpeedMetersPerSecond * GetFieldOfViewMultiplier(); }

        [Inject]
        public ZoomPositionRotationDHVController(
            [Inject(Id = "building")] GameObject building,
            [Inject(Id = "dhvWrapper")] Transform dhvWrapper)
        {
            dollhouseViewWrapperTransform = dhvWrapper;
            DefineBuildingCenter(building);
        }

        public bool moveInDirection(Vector2 direction, float speed)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * direction.x + cameraForward * direction.y;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);

            float movementDistance = keyboardMoveCameraRelativeSpeed * Time.deltaTime;
            Vector3 nextPosition = dollhouseViewWrapperTransform.position + (normalizedDirection * movementDistance);

            return PerformMovement(nextPosition);
        }


        public float GetFieldOfViewMultiplier()
        {
            return Camera.main.fieldOfView / baseFieldOfView;
        }

        void DefineBuildingCenter(GameObject building)
        {
            buildingCenter = BoundariesUtils.CalculateCenter(building);
        }

        bool PerformMovement(Vector3 nextPosition)
        {
            if (!isNextPositionInsideBoundaries(nextPosition))
            {
                return false;
            }
            dollhouseViewWrapperTransform.position = nextPosition;
            return true;
        }

        bool isNextPositionInsideBoundaries(Vector3 positionToCheck)
        {
            Vector3 offsetFromCenter = positionToCheck - buildingCenter;

            bool withinXBounds = Mathf.Abs(offsetFromCenter.x) <= limitDistanceFromBuildingInMeters;
            bool withinZBounds = Mathf.Abs(offsetFromCenter.z) <= limitDistanceFromBuildingInMeters;

            return withinXBounds && withinZBounds;
        }
        void SetStartingFocusRay(Vector2 pointerPosition)
        {
            Debug.Log($"79:setting starting pointerPosition >>>\n{pointerPosition}");
            startingFocusRay = RayUtils.GetRayFromCameraToScreenPoint(Camera.main, pointerPosition);
            hitPoint = RayUtils.GetHitPointDefaultToGround(this.startingFocusRay);
            originalCameraPosition = this.startingFocusRay.origin;
            originalWrapperPosition = dollhouseViewWrapperTransform.position;
        }

        void UpdateFocusRay(Vector2 pointerPosition)
        {
            // Debug.Log($"89: updating focus pointerPosition >>>\n{pointerPosition}");
            focusRay = RayUtils.GetRayFromCameraToScreenPoint(Camera.main, pointerPosition);
            Debug.DrawRay(focusRay.origin, focusRay.direction * 100, Color.green);
            Ray invertedRay = new Ray(hitPoint, -this.focusRay.direction);
            Vector3 newCameraPosition = RayUtils.ProjectRayToHeight(invertedRay, originalCameraPosition.y);
            Vector3 newWrapperPosition = originalWrapperPosition + (newCameraPosition - originalCameraPosition);
            PerformMovement(newWrapperPosition);
        }

    }

}