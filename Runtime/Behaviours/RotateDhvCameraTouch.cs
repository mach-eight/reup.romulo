using ReupVirtualTwin.enums;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class RotateDhvCameraTouch : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapper;

        InputProvider inputProvider;
        IGesturesManager gesturesManager;

        Vector2 initialTouch1Position;
        Vector2 initialTouch2Position;

        Vector2 touch1Position;
        Vector2 touch2Position;

        float rotationThreshold = 3f;

        float currentAngle;

        bool isHorizontalRotation = false;
        bool isVerticalRotation = false;

        [Inject]
        public void Init(InputProvider inputProvider, IGesturesManager gesturesManager)
        {
            this.inputProvider = inputProvider;
            this.gesturesManager = gesturesManager;
        }

        private void OnEnable() 
        {
            inputProvider.touch1Started += OnGestureStarted;
            inputProvider.touch1Canceled += OnGestureFinished;    
        }

        private void OnDisable() 
        {
            inputProvider.touch1Started -= OnGestureStarted;
            inputProvider.touch1Canceled -= OnGestureFinished;
        }

        private void Update() 
        {
            if (gesturesManager.gestureInProgress) 
            {
                if (!isHorizontalRotation && !isVerticalRotation)
                {
                    DetermineRotationType();
                }

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

        private void OnGestureStarted(InputAction.CallbackContext ctx)
        {
            initialTouch1Position = inputProvider.Touch0();
            initialTouch2Position = inputProvider.Touch1();
            touch1Position = initialTouch1Position;
            touch2Position = initialTouch2Position;
            currentAngle = Vector2.SignedAngle(touch2Position - touch1Position, Vector2.right);
        }

        private void OnGestureFinished(InputAction.CallbackContext ctx)
        {
            isHorizontalRotation = false;
            isVerticalRotation = false;
        }

        private void DetermineRotationType()
        {
            Vector2 touch1 = inputProvider.Touch0();
            Vector2 touch2 = inputProvider.Touch1();

            float angleDelta = Vector2.SignedAngle(touch2 - touch1, Vector2.right) - currentAngle;
            float newAverageY = (touch1.y + touch2.y) / 2;
            float previousAverageY = (initialTouch1Position.y + initialTouch2Position.y) / 2;
            float verticalDelta = newAverageY - previousAverageY;

            if (Mathf.Abs(angleDelta) > rotationThreshold)
            {
                isHorizontalRotation = true;
            }
            else if (Mathf.Abs(verticalDelta) > rotationThreshold && AreTouchesMovingInSameDirection(touch1, touch2))
            {
                isVerticalRotation = true;
            }
        }

        private void UpdateHorizontalCameraRotation()
        {
            Vector2 touch1 = inputProvider.Touch0();
            Vector2 touch2 = inputProvider.Touch1();
            Debug.Log("Touch1: " + touch1 + " Touch2: " + touch2);

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
            bool bothMovingUp = touch1.y > touch1Position.y && touch2.y > touch2Position.y;
            bool bothMovingDown = touch1.y < touch1Position.y && touch2.y < touch2Position.y;
            return bothMovingUp || bothMovingDown;
        }
    }
}