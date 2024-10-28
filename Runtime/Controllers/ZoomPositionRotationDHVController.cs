using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.helpers;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.controllers
{
    public class ZoomPositionRotationDHVController : IZoomPositionRotationDHVController
    {
        public Ray focusRay
        {
            set
            {
                Debug.Log($"17: _focusRay >>>\n{_focusRay}");
                UpdateCameraPosition(value);
            }
        }
        Ray _focusRay;
        public Ray startingFocusRay
        {
            set
            {
                SetStartingFocusRay(value);
            }
        }
        Ray _startingFocusRay;
        Transform dollhouseViewWrapperTransform;
        float baseFieldOfView = 60; // todo: this is a magic number, it should be obtained from the camera in initialization
        Vector3 buildingCenter;
        float limitDistanceFromBuildingInMeters = 35;

        Vector3 hitPoint;
        Vector3 originalCameraPosition;
        Vector3 originalWrapperPosition;

        [SerializeField] public float KeyboardMoveCameraSpeedMetersPerSecond = 40;

        public ZoomPositionRotationDHVController(
            [Inject(Id = "dhvWrapper")] Transform dhvWrapper)
        {
            dollhouseViewWrapperTransform = dhvWrapper;
        }

        public bool moveInDirection(Vector2 direction, float speed)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * direction.x + cameraForward * direction.y;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);

            float movementDistance = GetKeyboardMoveCameraRelativeSpeed() * Time.deltaTime;
            Vector3 nextPosition = dollhouseViewWrapperTransform.position + (normalizedDirection * movementDistance);

            return PerformMovement(nextPosition);
        }

        public float GetKeyboardMoveCameraRelativeSpeed()
        {
            return KeyboardMoveCameraSpeedMetersPerSecond * GetFieldOfViewMultiplier();
        }

        public float GetFieldOfViewMultiplier()
        {
            return Camera.main.fieldOfView / baseFieldOfView;
        }

        private bool PerformMovement(Vector3 nextPosition)
        {
            if (!isNextPositionInsideBoundaries(nextPosition))
            {
                return false;
            }
            dollhouseViewWrapperTransform.position = nextPosition;
            return true;
        }

        private bool isNextPositionInsideBoundaries(Vector3 positionToCheck)
        {
            Vector3 offsetFromCenter = positionToCheck - buildingCenter;

            bool withinXBounds = Mathf.Abs(offsetFromCenter.x) <= limitDistanceFromBuildingInMeters;
            bool withinZBounds = Mathf.Abs(offsetFromCenter.z) <= limitDistanceFromBuildingInMeters;

            return withinXBounds && withinZBounds;
        }
        void SetStartingFocusRay(Ray startingFocusRay)
        {
            _startingFocusRay = startingFocusRay;
            hitPoint = RayUtils.GetHitPoint(_startingFocusRay);
            originalCameraPosition = _startingFocusRay.origin;
            originalWrapperPosition = dollhouseViewWrapperTransform.position;
        }

        void UpdateCameraPosition(Ray focusRay)
        {
            _focusRay = focusRay;
            Ray invertedRay = new Ray(hitPoint, -_focusRay.direction);
            Vector3 newCameraPosition = RayUtils.ProjectRayToHeight(invertedRay, originalCameraPosition.y);
            Vector3 newWrapperPosition = originalWrapperPosition + (newCameraPosition - originalCameraPosition);
            PerformMovement(newWrapperPosition);
        }

    }

}