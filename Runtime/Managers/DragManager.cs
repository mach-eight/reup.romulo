using UnityEngine;
using UnityEngine.InputSystem;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class DragManager : MonoBehaviour, IDragManager
    {
        [HideInInspector]
        public bool dragging { get; private set; } = false;
        [HideInInspector]
        public bool prevDragging { get; private set; } = false;

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
            if (_isHolding == true && dragging == false)
            {
                var pointer = _inputProvider.PointerInput();
                var distance = Vector2.Distance(pointer, _selectPosition);
                dragging = distance > _dragDistanceThreshold;
            }
        }

        private void OnHold(InputAction.CallbackContext obj)
        {
            _isHolding = true;
            _selectPosition = _inputProvider.PointerInput();
        }

        private void OnHoldCanceled(InputAction.CallbackContext obj)
        {
            _isHolding = false;
            dragging = false;
        }
    }
}