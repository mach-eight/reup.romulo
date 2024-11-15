using UnityEngine;
using ReupVirtualTwin.inputs;
using Unity.Cinemachine;
using ReupVirtualTwin.managerInterfaces;
using Zenject;
using ReupVirtualTwin.controllerInterfaces;

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

        private InputProvider inputProvider;
        private IDhvNavigationController dhvNavigationController;
        private IGesturesManager gesturesManager;

        private float initialPinchDistance = 0;
        private float targetFieldOfView;
        private float currentVelocity;

        private float pinchGestureDistanceThresholdPixels = 20f;
        private float SCROLL_STEP = 120;

        [Inject]
        public void Init(InputProvider inputProvider, IGesturesManager gesturesManager, IDhvNavigationController dhvNavigationController)
        {
            this.inputProvider = inputProvider;
            this.gesturesManager = gesturesManager;
            this.dhvNavigationController = dhvNavigationController;
        }

        private void Start()
        {
            targetFieldOfView = dhvCamera.Lens.FieldOfView;
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
            HandleZoomGesture();
            ScrollWheelUpdateZoom();
            ApplySmoothZoom();
        }

        private void OnGestureStarted()
        {
            Vector2 touch0 = inputProvider.Touch0();
            Vector2 touch1 = inputProvider.Touch1();
            initialPinchDistance = Vector2.Distance(touch0, touch1);
        }

        private void OnGestureFinished()
        {
            initialPinchDistance = 0;
            if (dhvNavigationController.isZooming)
            {
                dhvNavigationController.StopZoom();
            }
        }

        private void HandleZoomGesture()
        {
            if (gesturesManager.gestureInProgress && !dhvNavigationController.isRotating)
            {
                Vector2 touch0 = inputProvider.Touch0();
                Vector2 touch1 = inputProvider.Touch1();

                if (!dhvNavigationController.isZooming)
                {
                    DetectZoomGesture(touch0, touch1);
                } else 
                {
                    PinchGestureUpdateZoom(touch0, touch1);
                }
            }
        }

        private void DetectZoomGesture(Vector2 touch0, Vector2 touch1)
        {
            float currentDistance = Vector2.Distance(touch0, touch1);

            if (Mathf.Abs(currentDistance - initialPinchDistance) > pinchGestureDistanceThresholdPixels)
            {
                dhvNavigationController.Zoom();
            }
        }

        private void PinchGestureUpdateZoom(Vector2 touch0, Vector2 touch1)
        {
            float currentDistance = Vector2.Distance(touch0, touch1);
            float zoomFactor = currentDistance / initialPinchDistance;

            targetFieldOfView = Mathf.Clamp(targetFieldOfView / zoomFactor, minFieldOfView, maxFieldOfView);
            initialPinchDistance = currentDistance;
        }

        private void ScrollWheelUpdateZoom()
        {
            Vector2 scrollInput = inputProvider.ScrollWheelZoomDhvCamera();
            if (scrollInput == Vector2.zero)
            {
                return;
            }
            float clampedScrollInput = Mathf.Clamp(scrollInput.y, -SCROLL_STEP, SCROLL_STEP);
            float zoomAmount = clampedScrollInput * scrollZoomSpeedMultiplier;
            float exponentialZoom = targetFieldOfView * Mathf.Pow(scrollZoomScaleFactor, zoomAmount);
            targetFieldOfView = Mathf.Clamp(exponentialZoom, minFieldOfView, maxFieldOfView);
        }

        private void ApplySmoothZoom()
        {
            if (dhvCamera.Lens.FieldOfView == targetFieldOfView)
            {
                return;
            }
            dhvCamera.Lens.FieldOfView = Mathf.SmoothDamp(dhvCamera.Lens.FieldOfView, targetFieldOfView, ref currentVelocity, smoothTime);
        }

    }
}