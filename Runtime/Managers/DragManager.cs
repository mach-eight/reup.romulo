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
            this.inputProvider = inputProvider;
        }


        public void Initialize()
        {
            inputProvider.holdStarted += OnHold;
            inputProvider.holdCanceled += OnHoldCanceled;
        }

        public void Dispose()
        {
            inputProvider.holdStarted -= OnHold;
            inputProvider.holdCanceled -= OnHoldCanceled;
        }

        public void Tick()
        {
            prevDragging = dragging;
            if (_isHolding == true && dragging == false)
            {
                var pointer = inputProvider.PointerInput();
                // Debug.Log($"46: pointer >>>\n{pointer}");
                var distance = Vector2.Distance(pointer, _selectPosition);
                // Debug.Log($"48: distance >>>\n{distance}");
                dragging = distance > _dragDistanceThreshold;
            }
        }

        private void OnHold(InputAction.CallbackContext obj)
        {
            // Debug.Log("onHOld started");
            _isHolding = true;
            _selectPosition = inputProvider.PointerInput();
        }

        private void OnHoldCanceled(InputAction.CallbackContext obj)
        {
            // Debug.Log("onHOld canceled");
            _isHolding = false;
            dragging = false;
        }
    }
}