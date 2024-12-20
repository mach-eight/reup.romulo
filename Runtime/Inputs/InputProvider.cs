using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.inputs
{
    public class InputProvider : IInitializable, IDisposable
    {
        AppInputActions _input;

        public InputProvider()
        {
            _input = new AppInputActions();
        }

        public void Initialize()
        {
            _input.Player.Enable();
            _input.DollhouseView.Enable();
            _input.MultiTouch.Enable();
        }

        public void Dispose()
        {
            _input.Player.Disable();
            _input.DollhouseView.Disable();
            _input.MultiTouch.Disable();
        }

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
        public event Action<InputAction.CallbackContext> holdRightClickStarted
        {
            add
            {
                _input.Player.HoldRightClick.started += value;
            }
            remove
            {
                _input.Player.HoldRightClick.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> holdRightClickPerformed
        {
            add
            {
                _input.Player.HoldRightClick.performed += value;
            }
            remove
            {
                _input.Player.HoldRightClick.performed -= value;
            }
        }
        public event Action<InputAction.CallbackContext> holdRightClickCanceled
        {
            add
            {
                _input.Player.HoldRightClick.canceled += value;
            }
            remove
            {
                _input.Player.HoldRightClick.canceled -= value;
            }
        }
        public event Action<InputAction.CallbackContext> touch0Started
        {
            add
            {
                _input.MultiTouch.Touch0.started += value;
            }
            remove
            {
                _input.MultiTouch.Touch0.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> touch0Canceled
        {
            add
            {
                _input.MultiTouch.Touch0.canceled += value;
            }
            remove
            {
                _input.MultiTouch.Touch0.canceled -= value;
            }
        }
        public event Action<InputAction.CallbackContext> touch1Started
        {
            add
            {
                _input.MultiTouch.Touch1.started += value;
            }
            remove
            {
                _input.MultiTouch.Touch1.started -= value;
            }
        }
        public event Action<InputAction.CallbackContext> touch1Canceled
        {
            add
            {
                _input.MultiTouch.Touch1.canceled += value;
            }
            remove
            {
                _input.MultiTouch.Touch1.canceled -= value;
            }
        }

        public Vector2 Touch0()
        {
            return _input.MultiTouch.Touch0.ReadValue<Vector2>();
        }
        public Vector2 Touch1()
        {
            return _input.MultiTouch.Touch1.ReadValue<Vector2>();
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
        public Vector2 ScrollWheelZoomDhvCamera()
        {
            return _input.DollhouseView.ScrollWheelZoom.ReadValue<Vector2>();
        }

        public Vector2 KeyboardRotateDhvCamera()
        {
            return _input.DollhouseView.KeyboardRotateCamera.ReadValue<Vector2>();
        }

        public Vector2 MouseRotateDhvCamera()
        {
            return _input.DollhouseView.MouseRotateCamera.ReadValue<Vector2>();
        }
    }
}