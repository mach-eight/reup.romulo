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
        public bool primaryDragging { get; private set; } = false;
        public bool secondaryDragging { get; private set; } = false;
        public bool prevDragging { get; private set; } = false;

        private bool isHoldingPrimaryDrag = false;
        private bool isHoldingSecondaryDrag = false;
        private Vector2 selectPositionPrimaryDrag;
        private Vector2 selectPositionSecondaryDrag;
        private InputProvider inputProvider;
        private float dragDistanceThreshold = 2.0f;

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
            if (isHoldingPrimaryDrag && !primaryDragging)
            {
                primaryDragging = isDragging(currentPointerPosition, selectPositionPrimaryDrag);
            }
            
            if (isHoldingSecondaryDrag && !secondaryDragging)
            {
                secondaryDragging = isDragging(currentPointerPosition, selectPositionSecondaryDrag);
            }
        }

        private bool isDragging(Vector2 pointerPosition, Vector2 selectPosition)
        {
            float distance = Vector2.Distance(pointerPosition, selectPosition);
            return distance > dragDistanceThreshold;
        }

        private void OnHold(InputAction.CallbackContext obj)
        {
            isHoldingPrimaryDrag = true;
            selectPositionPrimaryDrag = inputProvider.PointerInput();
        }

        private void OnHoldRightClick(InputAction.CallbackContext obj)
        {
            isHoldingSecondaryDrag = true;
            selectPositionSecondaryDrag = inputProvider.PointerInput();
        }

        private void OnHoldCanceled(InputAction.CallbackContext obj)
        {
            isHoldingPrimaryDrag = false;
            primaryDragging = false;
        }

        private void OnHoldRightClickCanceled(InputAction.CallbackContext obj)
        {
            isHoldingSecondaryDrag = false;
            secondaryDragging = false;
        }

    }
}