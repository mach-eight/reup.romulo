using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReupVirtualTwin.inputs
{
    public class InputProvider
    {
        private static AppInputActions _input = new ();

        public event Action<InputAction.CallbackContext> selectStarted
        {
            add
            {
                _input.Player.Select.started += value;
            }
            remove
            {
                _input.Player.Select.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> selectPerformed
        {
            add
            {
                _input.Player.Select.performed += value;
            }
            remove
            {
                _input.Player.Select.performed -= value;
            }
        }
        public event Action<InputAction.CallbackContext> selectCanceled
        {
            add
            {
                _input.Player.Select.canceled += value;
            }
            remove
            {
                _input.Player.Select.canceled -= value;
            }
        }

        public event Action<InputAction.CallbackContext> holdStarted
        {
            add
            {
                _input.Player.Hold.started += value;
            }
            remove
            {
                _input.Player.Hold.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> holdPerformed
        {
            add
            {
                _input.Player.Hold.performed += value;
            }
            remove
            {
                _input.Player.Hold.performed -= value;
            }
        }
        public event Action<InputAction.CallbackContext> holdCanceled
        {
            add
            {
                _input.Player.Hold.canceled += value;
            }
            remove
            {
                _input.Player.Hold.canceled -= value;
            }
        }
        public event Action<InputAction.CallbackContext> dhvZoomRotationTouchPerformedT1
        {
            add { _input.Player.DHVZoomRotationTouchT1.performed += value; }
            remove { _input.Player.DHVZoomRotationTouchT1.performed -= value; }
        }

        public event Action<InputAction.CallbackContext> dhvZoomRotationTouchStartedT1
        {
            add { _input.Player.DHVZoomRotationTouchT1.started += value; }
            remove { _input.Player.DHVZoomRotationTouchT1.started -= value; }
        }

        public event Action<InputAction.CallbackContext> dhvZoomRotationTouchCanceledT1
        {
            add { _input.Player.DHVZoomRotationTouchT1.canceled += value; }
            remove { _input.Player.DHVZoomRotationTouchT1.canceled -= value; }
        }
        public event Action<InputAction.CallbackContext> dhvZoomRotationTouchPerformedT2
        {
            add { _input.Player.DHVZoomRotationTouchT2.performed += value; }
            remove { _input.Player.DHVZoomRotationTouchT2.performed -= value; }
        }

        public event Action<InputAction.CallbackContext> dhvZoomRotationTouchStartedT2
        {
            add { _input.Player.DHVZoomRotationTouchT2.started += value; }
            remove { _input.Player.DHVZoomRotationTouchT2.started -= value; }
        }

        public event Action<InputAction.CallbackContext> dhvZoomRotationTouchCanceledT2
        {
            add { _input.Player.DHVZoomRotationTouchT2.canceled += value; }
            remove { _input.Player.DHVZoomRotationTouchT2.canceled -= value; }
        }
        public void Enable()
        {
            _input.Player.Enable();
        }

        public void Disable()
        {
            _input.Player.Disable();
        }

        public Vector2 RotateViewInput()
        {
            return _input.Player.RotateView.ReadValue<Vector2>() * -1;
        }
        public Vector2 RotateViewKeyboardInput()
        {
            return _input.Player.RotateViewKeyborad.ReadValue<Vector2>();
        }

        public Vector2 MovementInput()
        { 
            return _input.Player.Movement.ReadValue<Vector2>();
        }

        public Vector2 PointerInput()
        {
            return _input.Player.Pointer.ReadValue<Vector2>();
        }
        public float ChangeHeightInput()
        {
            return _input.Player.ChangeHeight.ReadValue<float>();
        }

        public Vector2 DhvMovementInput()
        {
            return _input.Player.DHVMovement.ReadValue<Vector2>();
        }

        public Vector2 DhvZoomInput()
        {
            return _input.Player.DHVZoom.ReadValue<Vector2>();
        }

        public Vector2 DhvRotationInput()
        {
            return _input.Player.DHVRotateKeyboard.ReadValue<Vector2>();
        }

        public Vector2 DhvMovementMouseAndTouch()
        {
            return _input.Player.DHVMovementMouseTouch.ReadValue<Vector2>();
        }

        public Vector2 DhvZoomRotationTouchT1Input()
        {
            return _input.Player.DHVZoomRotationTouchT1.ReadValue<Vector2>();
        }

        public Vector2 DhvZoomRotationTouchT2Input()
        {
            return _input.Player.DHVZoomRotationTouchT2.ReadValue<Vector2>();
        }

        public InputActionPhase DhvZoomRotationTouchT2InputPhase()
        {
            return _input.Player.DHVZoomRotationTouchT2.phase;
        }
    }
}