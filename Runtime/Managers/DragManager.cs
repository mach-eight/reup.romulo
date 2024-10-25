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
        [HideInInspector]
        public bool primaryDragging { get; private set; } = false;
        [HideInInspector]
        public bool secondaryDragging { get; private set; } = false;
        [HideInInspector]
        public bool prevDragging { get; private set; } = false;

        private bool _isHoldingPrimaryDrag = false;
        private bool _isHoldingSecondaryDrag = false;
        private Vector2 _selectPositionPrimaryDrag;
        private Vector2 _selectPositionSecondaryDrag;
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
            inputProvider.holdRightClickStarted += OnHoldRightClick;
            inputProvider.holdRightClickCanceled += OnHoldRightClickCanceled;
        }

        public void Dispose()
        {
            inputProvider.holdStarted -= OnHold;
            inputProvider.holdCanceled -= OnHoldCanceled;
            inputProvider.holdRightClickStarted -= OnHoldRightClick;
            inputProvider.holdRightClickCanceled -= OnHoldRightClickCanceled;
        }

        public void Tick()
        {
            prevDragging = primaryDragging || secondaryDragging;
            Vector2 currentPointerPosition = inputProvider.PointerInput();
            if (_isHoldingPrimaryDrag && !primaryDragging)
            {
                primaryDragging = isDragging(currentPointerPosition, _selectPositionPrimaryDrag);
            }
            
            if (_isHoldingSecondaryDrag && !secondaryDragging)
            {
                secondaryDragging = isDragging(currentPointerPosition, _selectPositionSecondaryDrag);
            }
        }

        private bool isDragging(Vector2 pointerPosition, Vector2 selectPosition)
        {
            float distance = Vector2.Distance(pointerPosition, selectPosition);
            return distance > _dragDistanceThreshold;
        }

        private void OnHold(InputAction.CallbackContext obj)
        {
            _isHoldingPrimaryDrag = true;
            _selectPositionPrimaryDrag = inputProvider.PointerInput();
        }

        private void OnHoldRightClick(InputAction.CallbackContext obj)
        {
            _isHoldingSecondaryDrag = true;
            _selectPositionSecondaryDrag = inputProvider.PointerInput();
        }

        private void OnHoldCanceled(InputAction.CallbackContext obj)
        {
            _isHoldingPrimaryDrag = false;
            primaryDragging = false;
        }

        private void OnHoldRightClickCanceled(InputAction.CallbackContext obj)
        {
            _isHoldingSecondaryDrag = false;
            secondaryDragging = false;
        }

    }
}