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

        InputProvider _inputProvider;
        IGesturesManager gesturesManager;

        Vector2 touch1Position;
        Vector2 touch2Position;
        float currentAngle;

        bool isInitDataSet = false;

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
                    return;
                }
                UpdateHorizontalCameraRotation();
                UpdateVerticalCameraRotation();
            }
            else 
            {
                isInitDataSet = false;
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

            if (!areTouchesMovingInSameDirection(touch1, touch2))
            {
                return;
            }

            float newAverageY = (touch1.y + touch2.y) / 2;
            float previousAverageY = (touch1Position.y + touch2Position.y) / 2;
            float deltaY = newAverageY - previousAverageY;
            float verticalRotation = (180f / Screen.height) * deltaY;

            float currentVerticalAngle = dollhouseViewWrapper.localEulerAngles.x;
            float newVerticalAngle = Mathf.Clamp(currentVerticalAngle - verticalRotation, minVerticalAngle, maxVerticalAngle);

            touch1Position = touch1;
            touch2Position = touch2;

            dollhouseViewWrapper.localEulerAngles = new Vector3(
                newVerticalAngle,
                dollhouseViewWrapper.localEulerAngles.y,
                0
            );
        }

        private void SetInitialData()
        {
            touch1Position = _inputProvider.Touch1Position();
            touch2Position = _inputProvider.Touch2Position();
            currentAngle = Vector2.SignedAngle(touch2Position - touch1Position, Vector2.right);
            isInitDataSet = true;
        }

        private bool areTouchesMovingInSameDirection(Vector2 touch1, Vector2 touch2)
        {
            bool bothMovingUp = touch1.y > touch1Position.y && touch2.y > touch2Position.y;
            bool bothMovingDown = touch1.y < touch1Position.y && touch2.y < touch2Position.y;
            return bothMovingUp || bothMovingDown;
        }

    }
}