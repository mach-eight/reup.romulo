using UnityEngine;
using ReupVirtualTwin.inputs;
using Unity.Cinemachine;
using ReupVirtualTwin.managerInterfaces;
using System;

namespace ReupVirtualTwin.behaviours
{
    public class ZoomDhvCamera : MonoBehaviour
    {
        [SerializeField] public CinemachineCamera dhvCamera;
        [SerializeField] public GameObject gesturesManagerGameObject;
        [SerializeField] public float scrollZoomSpeedMultiplier = 0.1f;
        [SerializeField] public float scrollZoomScaleFactor = 0.975f;
        [SerializeField] public float maxFieldOfView = 60;
        [SerializeField] public float minFieldOfView = 1;
        [SerializeField] public float smoothTime = 0.2f;

        private InputProvider _inputProvider;
        private IGesturesManager gesturesManager;
        
        private float initialPinchDistance = 0;
        private float targetFieldOfView;
        private float currentVelocity;

        private void Awake()
        {
            _inputProvider = new InputProvider();
            gesturesManager = gesturesManagerGameObject.GetComponent<IGesturesManager>();
            targetFieldOfView = dhvCamera.Lens.FieldOfView;
        }

        private void Update()
        {
            PinchGestureUpdateZoom();
            ScrollWheelUpdateZoom();
            ApplySmoothZoom();
        }

        private void PinchGestureUpdateZoom()
        {
            if (!gesturesManager.gestureInProgress)
            {
                initialPinchDistance = 0;
                return;
            }

            Vector2 touch1 = _inputProvider.Touch1Position();
            Vector2 touch2 = _inputProvider.Touch2Position();
            
            if (initialPinchDistance == 0)
            {
                initialPinchDistance = Vector2.Distance(touch1, touch2);
            }

            float currentDistance = Vector2.Distance(touch1, touch2);
            float zoomFactor = currentDistance / initialPinchDistance;

            targetFieldOfView = Mathf.Clamp(targetFieldOfView / zoomFactor, minFieldOfView, maxFieldOfView);
            initialPinchDistance = currentDistance;
        }

        private void ScrollWheelUpdateZoom()
        {
            Vector2 scrollInput = _inputProvider.ScrollWheelZoomDhvCamera();
            if (scrollInput == Vector2.zero)
            {
                return;
            }
            float zoomAmount = scrollInput.y * scrollZoomSpeedMultiplier;
            float exponentialZoom = targetFieldOfView * Mathf.Pow(scrollZoomScaleFactor, zoomAmount);
            targetFieldOfView = Mathf.Clamp(exponentialZoom, minFieldOfView, maxFieldOfView);
        }

        private void ApplySmoothZoom()
        {
            dhvCamera.Lens.FieldOfView = Mathf.SmoothDamp(dhvCamera.Lens.FieldOfView, targetFieldOfView, ref currentVelocity, smoothTime);
        }

    }
}