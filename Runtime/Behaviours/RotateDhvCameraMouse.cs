using ReupVirtualTwin.enums;
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
        InputProvider inputProvider;
        IDragManager dragManager;

        [Inject]
        public void Init(IDragManager dragManager, InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
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
            Vector2 mouseDelta = inputProvider.MouseRotateDhvCamera();

            float horizontalRotation = (mouseDelta.x / Screen.width) * RomuloGlobalSettings.horizontalRotationPerScreenWidth;
            float verticalRotation = (mouseDelta.y / Screen.height) * RomuloGlobalSettings.verticalRotationPerScreenHeight;

            float currentVerticalAngle = dollhouseViewWrapper.localEulerAngles.x;
            float newVerticalAngle = Mathf.Clamp(
                currentVerticalAngle - verticalRotation, 
                RomuloGlobalSettings.minVerticalAngle, 
                RomuloGlobalSettings.maxVerticalAngle
            );

            dollhouseViewWrapper.localEulerAngles = new Vector3(
                newVerticalAngle,
                dollhouseViewWrapper.localEulerAngles.y + horizontalRotation,
                0
            );
        }
    }
}