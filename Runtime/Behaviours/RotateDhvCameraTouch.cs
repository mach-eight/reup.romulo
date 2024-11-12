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

        Vector2 initialTouch1Position;
        Vector2 initialTouch2Position;

        Vector2 touch1Position;
        Vector2 touch2Position;

        float horizontalRotationThreshold = 6f;
        float verticalRotationThreshold = 5f;
        float movementDirectionThreshold = 2f;

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
            if (gesturesManager.gestureInProgress) 
            {
                if (!isHorizontalRotation && !isVerticalRotation)
                {
                    DetermineRotationType();
                }

                if (dhvNavigationController.isRotating)
                {
                    if (isHorizontalRotation)
                    {
                        UpdateHorizontalCameraRotation();
                    }
                    else if (isVerticalRotation)
                    {
                        UpdateVerticalCameraRotation();
                    }
                }
            }
        }

        private void OnGestureStarted()
        {
            initialTouch1Position = inputProvider.Touch0();
            initialTouch2Position = inputProvider.Touch1();
            touch1Position = initialTouch1Position;
            touch2Position = initialTouch2Position;
            currentAngle = Vector2.SignedAngle(touch2Position - touch1Position, Vector2.right);
        }

        private void OnGestureFinished()
        {
            isHorizontalRotation = false;
            isVerticalRotation = false;
            if (dhvNavigationController.isRotating)
            {
                dhvNavigationController.StopNavigationAction();
            }
        }

        private void DetermineRotationType()
        {
            Vector2 touch1 = inputProvider.Touch0();
            Vector2 touch2 = inputProvider.Touch1();

            float angleDelta = Vector2.SignedAngle(touch2 - touch1, Vector2.right) - currentAngle;
            float newAverageY = (touch1.y + touch2.y) / 2;
            float previousAverageY = (initialTouch1Position.y + initialTouch2Position.y) / 2;
            float verticalDelta = newAverageY - previousAverageY;

            if (Mathf.Abs(angleDelta) > horizontalRotationThreshold)
            {
                isHorizontalRotation = true;
            }
            else if (Mathf.Abs(verticalDelta) > verticalRotationThreshold && AreTouchesMovingInSameDirection(touch1, touch2))
            {
                isVerticalRotation = true;
            }

            if (isHorizontalRotation || isVerticalRotation)
            {
                dhvNavigationController.Rotate();
            }
        }

        private void UpdateHorizontalCameraRotation()
        {
            Vector2 touch1 = inputProvider.Touch0();
            Vector2 touch2 = inputProvider.Touch1();

            float newAngle = Vector2.SignedAngle(touch2 - touch1, Vector2.right);
            float angleDelta = newAngle - currentAngle;

            dollhouseViewWrapper.Rotate(Vector3.up, -angleDelta, Space.World);

            currentAngle = newAngle;
        }

        private void UpdateVerticalCameraRotation()
        {
            Vector2 touch1 = inputProvider.Touch0();
            Vector2 touch2 = inputProvider.Touch1();

            if (!AreTouchesMovingInSameDirection(touch1, touch2))
            {
                return;
            }

            float newAverageY = (touch1.y + touch2.y) / 2;
            float previousAverageY = (touch1Position.y + touch2Position.y) / 2;
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

            touch1Position = touch1;
            touch2Position = touch2;
        }

        private bool AreTouchesMovingInSameDirection(Vector2 touch1, Vector2 touch2)
        {
            bool bothMovingUp = touch1.y > touch1Position.y + movementDirectionThreshold && touch2.y > touch2Position.y + movementDirectionThreshold;
            bool bothMovingDown = touch1.y < touch1Position.y - movementDirectionThreshold && touch2.y < touch2Position.y - movementDirectionThreshold;
            return bothMovingUp || bothMovingDown;
        }
    }
}