using ReupVirtualTwin.enums;
using ReupVirtualTwin.inputs;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class RotateDhvCameraKeyboard : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapper;
        [SerializeField] public float keyboardRotationSpeedDegreesPerSecond = 80f;

        private InputProvider inputProvider;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        private void Update()
        {
            Vector2 rotationDirection = inputProvider.KeyboardRotateDhvCamera();
            if (rotationDirection == Vector2.zero)
            {
                return;
            }
            float deltaSpeed = keyboardRotationSpeedDegreesPerSecond * Time.deltaTime;
            float horizontalRotation = dollhouseViewWrapper.localEulerAngles.y - (rotationDirection.x * deltaSpeed);
            float verticalRotation = dollhouseViewWrapper.localEulerAngles.x - (rotationDirection.y * deltaSpeed * -1);
            verticalRotation = Mathf.Clamp(verticalRotation, RomuloGlobalSettings.minVerticalAngle, RomuloGlobalSettings.maxVerticalAngle);
            dollhouseViewWrapper.localEulerAngles = new Vector3(verticalRotation, horizontalRotation, 0);
        }

    }

}
