using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class RotateDhvCameraTouch : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapper;

        InputProvider inputProvider;
        IGesturesManager gesturesManager;
        IDhvNavigationController dhvNavigationController;

        Vector2 initialTouch0Position;
        Vector2 initialTouch1Position;

        Vector2 touch0Position;
        Vector2 touch1Position;

        float horizontalRotationThresholdDegrees = 6f;
        float verticalRotationThresholdPixels = 5f;
        float movementDirectionThresholdPixels = 2f;

        float currentAngle;

        bool isHorizontalRotation = false;
        bool isVerticalRotation = false;

        [Inject]
        public void Init(InputProvider inputProvider, IGesturesManager gesturesManager, IDhvNavigationController dhvNavigationController)
        {
            this.inputProvider = inputProvider;
            this.gesturesManager = gesturesManager;
            this.dhvNavigationController = dhvNavigationController;
        }

        private void OnEnable() 
        {
            gesturesManager.GestureStarted += OnGestureStarted;
            gesturesManager.GestureEnded += OnGestureFinished;    
        }

        private void OnDisable() 
        {
            gesturesManager.GestureStarted -= OnGestureStarted;
            gesturesManager.GestureEnded -= OnGestureFinished;
        }

        private void Update() 
        {
            if (gesturesManager.gestureInProgress && !dhvNavigationController.isZooming) 
            {
                Vector2 touch0 = inputProvider.Touch0();
                Vector2 touch1 = inputProvider.Touch1();

                if (!isHorizontalRotation && !isVerticalRotation)
                {
                    DetermineRotationType(touch0, touch1);
                }

                if (isHorizontalRotation || isVerticalRotation)
                {
                    dhvNavigationController.Rotate();
                }

                if (dhvNavigationController.isRotating)
                {
                    RotateCamera(touch0, touch1);
                }
            }
        }

        private void OnGestureStarted()
        {
            initialTouch0Position = inputProvider.Touch0();
            initialTouch1Position = inputProvider.Touch1();
            touch0Position = initialTouch0Position;
            touch1Position = initialTouch1Position;
            currentAngle = Vector2.SignedAngle(touch1Position - touch0Position, Vector2.right);
        }

        private void OnGestureFinished()
        {
            isHorizontalRotation = false;
            isVerticalRotation = false;
            if (dhvNavigationController.isRotating)
            {
                dhvNavigationController.StopRotation();
            }
        }

        private void DetermineRotationType(Vector2 touch0, Vector2 touch1)
        {
            float horizontalAngleDelta = Vector2.SignedAngle(touch1 - touch0, Vector2.right) - currentAngle;
            float newAverageY = (touch0.y + touch1.y) / 2;
            float previousAverageY = (initialTouch0Position.y + initialTouch1Position.y) / 2;
            float verticalDelta = newAverageY - previousAverageY;

            if (Mathf.Abs(horizontalAngleDelta) > horizontalRotationThresholdDegrees)
            {
                isHorizontalRotation = true;
            }
            else if (Mathf.Abs(verticalDelta) > verticalRotationThresholdPixels && AreTouchesMovingInSameDirection(touch0, touch1))
            {
                isVerticalRotation = true;
            }
        }

        private void RotateCamera(Vector2 touch0, Vector2 touch1)
        {
            if (isHorizontalRotation)
            {
                UpdateHorizontalCameraRotation(touch0, touch1);
            }
            else if (isVerticalRotation)
            {
                UpdateVerticalCameraRotation(touch0, touch1);
            }
        }

        private void UpdateHorizontalCameraRotation(Vector2 touch0, Vector2 touch1)
        {
            float newAngle = Vector2.SignedAngle(touch1 - touch0, Vector2.right);
            float angleDelta = newAngle - currentAngle;

            dollhouseViewWrapper.Rotate(Vector3.up, -angleDelta, Space.World);

            currentAngle = newAngle;
        }

        private void UpdateVerticalCameraRotation(Vector2 touch0, Vector2 touch1)
        {
            if (!AreTouchesMovingInSameDirection(touch0, touch1))
            {
                return;
            }

            float newAverageY = (touch0.y + touch1.y) / 2;
            float previousAverageY = (touch0Position.y + touch1Position.y) / 2;
            float deltaY = newAverageY - previousAverageY;
            float verticalRotation = (RomuloGlobalSettings.verticalRotationPerScreenHeight / Screen.height) * deltaY;

            float currentVerticalAngle = dollhouseViewWrapper.localEulerAngles.x;
            float newVerticalAngle = Mathf.Clamp(
                currentVerticalAngle - verticalRotation, 
                RomuloGlobalSettings.minVerticalAngle, 
                RomuloGlobalSettings.maxVerticalAngle
            );

            dollhouseViewWrapper.localEulerAngles = new Vector3(
                newVerticalAngle,
                dollhouseViewWrapper.localEulerAngles.y,
                0
            );

            touch0Position = touch0;
            touch1Position = touch1;
        }

        private bool AreTouchesMovingInSameDirection(Vector2 touch1, Vector2 touch2)
        {
            bool bothMovingUp = touch1.y > touch0Position.y + movementDirectionThresholdPixels && touch2.y > touch1Position.y + movementDirectionThresholdPixels;
            bool bothMovingDown = touch1.y < touch0Position.y - movementDirectionThresholdPixels && touch2.y < touch1Position.y - movementDirectionThresholdPixels;
            return bothMovingUp || bothMovingDown;
        }
    }
}