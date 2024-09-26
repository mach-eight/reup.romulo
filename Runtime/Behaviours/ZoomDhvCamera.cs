using UnityEngine;
using ReupVirtualTwin.inputs;
using Unity.Cinemachine;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.behaviours
{
    public class ZoomDhvCamera : MonoBehaviour
    {
        [SerializeField] public CinemachineCamera dhvCamera;
        [SerializeField] public float zoomSpeed = 0.1f;
        [SerializeField] public float maxFieldOfView = 60;
        [SerializeField] public float minFieldOfView = 1;
        [SerializeField] public float smoothTime = 0.2f;

        private InputProvider _inputProvider;
        private GesturesManager gesturesManager;
        
        private float initialPinchDistance = 0;
        private float targetFieldOfView;
        private float currentVelocity;

        private void Awake()
        {
            _inputProvider = new InputProvider();
            gesturesManager = ObjectFinder.FindGesturesManager().GetComponent<GesturesManager>();
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

            Vector2 touch1 = _inputProvider.GestureTouch1Dhv();
            Vector2 touch2 = _inputProvider.GestureTouch2Dhv();
            
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
            float zoomAmount = scrollInput.y * zoomSpeed;
            float exponentialZoom = targetFieldOfView * Mathf.Pow(0.975f, zoomAmount);
            targetFieldOfView = Mathf.Clamp(exponentialZoom, minFieldOfView, maxFieldOfView);
        }

        private void ApplySmoothZoom()
        {
            dhvCamera.Lens.FieldOfView = Mathf.SmoothDamp(dhvCamera.Lens.FieldOfView, targetFieldOfView, ref currentVelocity, smoothTime);
        }

    }
}