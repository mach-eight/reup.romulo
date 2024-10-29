using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class MoveDhvCamera : MonoBehaviour
    {
        [SerializeField] public float KeyboardMoveCameraSpeedMetersPerSecond = 40; //todo inject this from Reup prefab

        InputProvider _inputProvider;
        IDragManager dragManager;
        IGesturesManager gesturesManager;
        IZoomPositionRotationDHVController zoomPositionRotationDHVController;

        private void OnEnable()
        {
            _inputProvider.holdStarted += OnHoldStarted;
            _inputProvider.touch1Started += OnTouch1Started;
            _inputProvider.touch1Canceled += OnTouch1Ended;
        }
        private void OnDisable()
        {
            _inputProvider.holdStarted -= OnHoldStarted;
            _inputProvider.touch1Started -= OnTouch1Started;
            _inputProvider.touch1Canceled -= OnTouch1Ended;
        }

        void OnHoldStarted(InputAction.CallbackContext ctx)
        {
            Debug.Log("inside on hold started");
            LogScreenDimensions();
            zoomPositionRotationDHVController.startingFocusScreenPoint = _inputProvider.PointerInput();
        }
        void OnTouch1Started(InputAction.CallbackContext ctx)
        {
            zoomPositionRotationDHVController.startingFocusScreenPoint = GetMeanPointBetweenTouches();
        }
        void OnTouch1Ended(InputAction.CallbackContext ctx)
        {
            zoomPositionRotationDHVController.startingFocusScreenPoint = _inputProvider.PointerInput();
        }

        void LogScreenDimensions()
        {
            // Debug.Log("Screen dimensions: " + Screen.width + "x" + Screen.height);
        }
        Vector2 GetMeanPointBetweenTouches()
        {
            Vector2 firstTouchPosition = _inputProvider.Touch1Position();
            Vector2 secondTouchPosition = _inputProvider.Touch2Position();
            return (firstTouchPosition + secondTouchPosition) / 2;
        }

        [Inject]
        public void Init(
            IDragManager dragManager,
            InputProvider inputProvider,
            IGesturesManager gesturesManager,
            IZoomPositionRotationDHVController zoomPositionRotationDHVController)
        {
            _inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.zoomPositionRotationDHVController = zoomPositionRotationDHVController;
            this.gesturesManager = gesturesManager;
        }

        void Update()
        {
            // Debug.Log($"73: _inputProvider.Touch2Position() >>>\n{_inputProvider.Touch2Position()}");
            Ray touch0Ray = Camera.main.ScreenPointToRay(_inputProvider.Touch1Position());
            Debug.DrawRay(touch0Ray.origin, touch0Ray.direction * 100, Color.red);
            Ray touch1Ray = Camera.main.ScreenPointToRay(_inputProvider.Touch2Position());
            Debug.DrawRay(touch1Ray.origin, touch1Ray.direction * 100, Color.blue);
            KeyboardUpdatePosition();
            PointerUpdatePosition();
        }

        void KeyboardUpdatePosition()
        {
            Vector2 inputValue = _inputProvider.KeyboardMoveDhvCamera().normalized;
            zoomPositionRotationDHVController.moveInDirection(inputValue, KeyboardMoveCameraSpeedMetersPerSecond);
        }

        void PointerUpdatePosition()
        {
            if (!dragManager.dragging && !gesturesManager.gestureInProgress)
            {
                return;
            }
            zoomPositionRotationDHVController.focusScreenPoint = GetFocusPoint();
        }

        Vector2 GetFocusPoint()
        {

            if (gesturesManager.gestureInProgress)
            {
                return GetMeanPointBetweenTouches();
            }
            return _inputProvider.PointerInput();
        }

    }
}
