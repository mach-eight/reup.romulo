using UnityEngine;
using UnityEngine.InputSystem;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using Zenject;
using System;
using UnityEditor;

namespace ReupVirtualTwin.managers
{
    public class DragManager : IDragManager, IInitializable, ITickable, IDisposable
    {
        public bool dragging { get; private set; } = false;
        public bool prevDragging { get; private set; } = false;

        private bool _isHolding = false;
        private Vector2 _selectPosition;
        private InputProvider inputProvider;
        private float _dragDistanceThreshold = 2.0f;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            Debug.Log("in drag manager init");
            this.inputProvider = inputProvider;
            Debug.Log($"25: this.inputProvider >>>\n{this.inputProvider}");
        }


        public void Initialize()
        {
            Debug.Log("in drag manager initialize");
            // inputProvider = new InputProvider();
            // Debug.Log("in drag manager");
            // Debug.Log($"24: _inputProvider >>>\n{inputProvider}");
            inputProvider.holdStarted += OnHold;
            inputProvider.holdCanceled += OnHoldCanceled;
        }

        public void Dispose()
        {
            Debug.Log("disposing drag manager");
            inputProvider.holdStarted -= OnHold;
            inputProvider.holdCanceled -= OnHoldCanceled;
        }

        public void Tick()
        {
            prevDragging = dragging;
            if (_isHolding == true && dragging == false)
            {
                var pointer = inputProvider.PointerInput();
                var distance = Vector2.Distance(pointer, _selectPosition);
                dragging = distance > _dragDistanceThreshold;
            }
        }

        private void OnHold(InputAction.CallbackContext obj)
        {
            _isHolding = true;
            _selectPosition = inputProvider.PointerInput();
        }

        private void OnHoldCanceled(InputAction.CallbackContext obj)
        {
            _isHolding = false;
            dragging = false;
        }
    }
}