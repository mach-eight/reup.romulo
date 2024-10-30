using NUnit.Framework;
using NUnit.Framework.Internal;
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
        [SerializeField] public float maxVerticalAngle = 89.9f;
        [SerializeField] public float minVerticalAngle = 10f;
        [SerializeField] public float rotationThreshold = 2.5f;
        [SerializeField] public float verticalRotationPerScreenHeight = 180f;

        InputProvider _inputProvider;
        IGesturesManager gesturesManager;

        Vector2 initialTouch1Position;
        Vector2 initialTouch2Position;

        Vector2 touch1Position;
        Vector2 touch2Position;
        float currentAngle;

        bool isInitDataSet = false;
        bool isHorizontalRotation = false;
        bool isVerticalRotation = false;

        [Inject]
        public void Init(InputProvider inputProvider, IGesturesManager gesturesManager)
        {
            _inputProvider = inputProvider;
            this.gesturesManager = gesturesManager;
        }

        private void Update() 
        {
            if (gesturesManager.gestureInProgress) 
            {
                if (!isInitDataSet)
                {
                    SetInitialData();
                }

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
            else 
            {
                ResetInitialData();
            }
        }

        private void SetInitialData()
        {
            initialTouch1Position = _inputProvider.Touch1Position();
            initialTouch2Position = _inputProvider.Touch2Position();
            touch1Position = initialTouch1Position;
            touch2Position = initialTouch2Position;
            currentAngle = Vector2.SignedAngle(touch2Position - touch1Position, Vector2.right);
            isInitDataSet = true;
        }

        private void ResetInitialData()
        {
            isInitDataSet = false;
            isHorizontalRotation = false;
            isVerticalRotation = false;
        }

        private void DetermineRotationType()
        {
            Vector2 touch1 = _inputProvider.Touch1Position();
            Vector2 touch2 = _inputProvider.Touch2Position();

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
            Vector2 touch1 = _inputProvider.Touch1Position();
            Vector2 touch2 = _inputProvider.Touch2Position();

            float newAngle = Vector2.SignedAngle(touch2 - touch1, Vector2.right);
            float angleDelta = newAngle - currentAngle;

            dollhouseViewWrapper.Rotate(Vector3.up, -angleDelta, Space.World);

            currentAngle = newAngle;
        }

        private void UpdateVerticalCameraRotation()
        {
            Vector2 touch1 = _inputProvider.Touch1Position();
            Vector2 touch2 = _inputProvider.Touch2Position();

            if (!AreTouchesMovingInSameDirection(touch1, touch2))
            {
                return;
            }

            float newAverageY = (touch1.y + touch2.y) / 2;
            float previousAverageY = (touch1Position.y + touch2Position.y) / 2;
            float deltaY = newAverageY - previousAverageY;
            float verticalRotation = (verticalRotationPerScreenHeight / Screen.height) * deltaY;

            float currentVerticalAngle = dollhouseViewWrapper.localEulerAngles.x;
            float newVerticalAngle = Mathf.Clamp(currentVerticalAngle - verticalRotation, minVerticalAngle, maxVerticalAngle);

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