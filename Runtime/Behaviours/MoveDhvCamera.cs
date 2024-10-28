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
            // _inputProvider.touch2HoldStarted += OnSecondTouchStarted;
            _inputProvider.touch2HoldPerformed += OnSecondTouchStarted;
        }
        private void OnDisable()
        {
            _inputProvider.holdStarted -= OnHoldStarted;
            // _inputProvider.touch2HoldStarted -= OnSecondTouchStarted;
            _inputProvider.touch2HoldPerformed -= OnSecondTouchStarted;
        }

        void OnHoldStarted(InputAction.CallbackContext ctx)
        {
            Debug.Log("inside on hold started");
            LogScreenDimensions();
            zoomPositionRotationDHVController.startingFocusScreenPoint = _inputProvider.PointerInput();
        }
        void OnSecondTouchStarted(InputAction.CallbackContext ctx)
        {
            Debug.Log("inside on second touch started");
            zoomPositionRotationDHVController.startingFocusScreenPoint = GetMeanPointBetweenTouches();
        }

        void LogScreenDimensions()
        {
            Debug.Log("Screen dimensions: " + Screen.width + "x" + Screen.height);
        }
        Vector2 GetMeanPointBetweenTouches()
        {
            Debug.Log("getting the mean point between touches");
            Vector2 firstTouchPosition = _inputProvider.Touch1Position();
            Debug.Log($"49: firstTouchPosition >>>\n{firstTouchPosition}");
            Vector2 secondTouchPosition = _inputProvider.Touch2Position();
            Debug.Log($"51: secondTouchPosition >>>\n{secondTouchPosition}");
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
            Debug.Log($"73: _inputProvider.Touch2Position() >>>\n{_inputProvider.Touch2Position()}");
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
                // Debug.Log("sorry doing nothing");
                return;
            }
            zoomPositionRotationDHVController.focusScreenPoint = GetFocusPoint();
        }

        Vector2 GetFocusPoint()
        {

            if (gesturesManager.gestureInProgress)
            {
                Debug.Log("gesture in progress");
                return GetMeanPointBetweenTouches();
            }
            return _inputProvider.PointerInput();
        }

    }
}
