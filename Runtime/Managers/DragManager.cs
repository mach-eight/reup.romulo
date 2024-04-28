using UnityEngine;
using UnityEngine.InputSystem;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;

namespace ReupVirtualTwin.managers
{
    public class DragManager : MonoBehaviour
    {
        [HideInInspector]
        public bool dragging = false;
        [HideInInspector]
        public bool prevDragging = false;
        [HideInInspector]
        public bool selectInputInUI = false;
        [HideInInspector]
        public bool prevSelectInputInUI = false;

        private bool _isHolding = false;
        private Vector2 _selectPosition;
        private InputProvider _inputProvider;
        private float _dragDistanceThreshold = 2.0f;

        private void Awake()
        {
            _inputProvider = new InputProvider();
        }

        private void OnEnable()
        {
            _inputProvider.holdStarted += OnHold;
            _inputProvider.holdCanceled += OnHoldCanceled;
        }

        private void OnDisable()
        {
            _inputProvider.holdStarted -= OnHold;
            _inputProvider.holdCanceled -= OnHoldCanceled;
        }

        void Update()
        {
            prevDragging = dragging;
            prevSelectInputInUI = selectInputInUI;
            if (_isHolding == true && dragging == false)
            {
                var pointer = _inputProvider.PointerInput();
                var distance = Vector2.Distance(pointer, _selectPosition);
                dragging = distance > _dragDistanceThreshold;
            }
        }

        private void OnHold(InputAction.CallbackContext obj)
        {
            if (OverUICheck.PointerOverUI())
            {
                selectInputInUI = true;
            }
            else
            {
                _isHolding = true;
                _selectPosition = _inputProvider.PointerInput();
            }
        }

        private void OnHoldCanceled(InputAction.CallbackContext obj)
        {
            _isHolding = false;
            dragging = false;
            selectInputInUI = false;
        }
    }
}