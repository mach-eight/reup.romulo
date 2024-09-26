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
        public event Action<InputAction.CallbackContext> firstTouchStarted
        {
            add
            {
                _input.MultiTouch.PrimaryTouch.started += value;
            }
            remove
            {
                _input.MultiTouch.PrimaryTouch.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> firstTouchCanceled
        {
            add
            {
                _input.MultiTouch.PrimaryTouch.canceled += value;
            }
            remove
            {
                _input.MultiTouch.PrimaryTouch.canceled -= value;
            }
        }
        public event Action<InputAction.CallbackContext> secondTouchStarted
        {
            add
            {
                _input.MultiTouch.SecondaryTouch.started += value;
            }
            remove
            {
                _input.MultiTouch.SecondaryTouch.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> secondTouchCanceled
        {
            add
            {
                _input.MultiTouch.SecondaryTouch.canceled += value;
            }
            remove
            {
                _input.MultiTouch.SecondaryTouch.canceled -= value;
            }
        }
        public void Enable()
        {
            _input.Player.Enable();
            _input.DollhouseView.Enable();
            _input.MultiTouch.Enable();
        }

        public void Disable()
        {
            _input.Player.Disable();
            _input.DollhouseView.Disable();
            _input.MultiTouch.Disable();
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
        public Vector2 KeyboardMoveDhvCamera()
        {
            return _input.DollhouseView.KeyboardMoveCamera.ReadValue<Vector2>();
        }
        public Vector2 PointerMoveDhvCamera()
        {
            return -1 * _input.DollhouseView.PointerMoveCamera.ReadValue<Vector2>();
        }
        public Vector2 GestureTouch1Dhv()
        {
            return _input.DollhouseView.GestureTouch1.ReadValue<Vector2>(); 
        }
        public Vector2 GestureTouch2Dhv()
        {
            return _input.DollhouseView.GestureTouch2.ReadValue<Vector2>(); 
        }
        public Vector2 ScrollWheelZoomDhvCamera()
        {
            return _input.DollhouseView.ScrollWheelZoom.ReadValue<Vector2>();
        }
    }
}