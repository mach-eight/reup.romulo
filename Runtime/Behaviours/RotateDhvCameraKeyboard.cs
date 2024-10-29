using ReupVirtualTwin.inputs;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class RotateDhvCameraKeyboard : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapper;
        [SerializeField] public float keyboardRotationSpeedDegreesPerSecond = 80f;
        [SerializeField] public float maxVerticalAngle = 89.9f;
        [SerializeField] public float minVerticalAngle = 10f;

        private InputProvider _inputProvider;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }

        private void Update()
        {
            UpdateCameraRotationWithKeyboard();
        }

        private void UpdateCameraRotationWithKeyboard()
        {
            Vector2 rotationDirection = _inputProvider.KeyboardRotateDhvCamera();
            if (rotationDirection == Vector2.zero)
            {
                return;
            }
            float deltaSpeed = keyboardRotationSpeedDegreesPerSecond * Time.deltaTime;
            float horizontalRotation = dollhouseViewWrapper.localEulerAngles.y - (rotationDirection.x * deltaSpeed);
            float verticalRotation = dollhouseViewWrapper.localEulerAngles.x - (rotationDirection.y * deltaSpeed * -1);
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
            dollhouseViewWrapper.localEulerAngles = new Vector3(verticalRotation, horizontalRotation, 0);
        }

    }

}
