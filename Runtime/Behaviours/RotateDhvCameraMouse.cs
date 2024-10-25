using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class RotateDhvCameraMouse : MonoBehaviour
    {
        [SerializeField] public Transform dollhouseViewWrapper;
        private float maxVerticalAngle = 89.9f;
        private float minVerticalAngle = 10f;
        InputProvider _inputProvider;
        IDragManager dragManager;

        [Inject]
        public void Init(IDragManager dragManager, InputProvider inputProvider)
        {
            _inputProvider = inputProvider;
            this.dragManager = dragManager;
        }
        private void Update()
        {
            if (dragManager.secondaryDragging)
            {
                UpdateCameraRotationWithMouse();
            }
        }

        private void UpdateCameraRotationWithMouse()
        {
            Vector2 mouseDelta = _inputProvider.MouseRotateDhvCamera();

            float horizontalRotation = (mouseDelta.x / Screen.width) * 180f;
            float verticalRotation = (mouseDelta.y / Screen.height) * 180f;

            float currentVerticalAngle = dollhouseViewWrapper.localEulerAngles.x;
            float newVerticalAngle = Mathf.Clamp(currentVerticalAngle - verticalRotation, minVerticalAngle, maxVerticalAngle);

            dollhouseViewWrapper.localEulerAngles = new Vector3(
                newVerticalAngle,
                dollhouseViewWrapper.localEulerAngles.y + horizontalRotation,
                0
            );
        }
    }
}