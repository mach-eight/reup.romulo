using System;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.managers
{
    public class GesturesManager : IGesturesManager, IInitializable, IDisposable
    {
        [HideInInspector] public bool gestureInProgress { get; private set; } = false;
        InputProvider inputProvider;
        private int activeTouchInputs = 0;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public void Initialize()
        {
            inputProvider.touch0Started += TouchStarted;
            inputProvider.touch0Canceled += TouchStopped;
            inputProvider.touch1Started += TouchStarted;
            inputProvider.touch1Canceled += TouchStopped;
        }

        public void Dispose()
        {
            inputProvider.touch0Started -= TouchStarted;
            inputProvider.touch0Canceled -= TouchStopped;
            inputProvider.touch1Started -= TouchStarted;
            inputProvider.touch1Canceled -= TouchStopped;
        }

        private void TouchStarted(InputAction.CallbackContext ctx)
        {
            Debug.Log("a touch started");
            activeTouchInputs++;
            gestureInProgress = activeTouchInputs > 1;
        }

        private void TouchStopped(InputAction.CallbackContext ctx)
        {
            Debug.Log("a touch ended");
            activeTouchInputs--;
            gestureInProgress = activeTouchInputs > 1;
        }

        // public void Tick()
        // {
        //     Debug.Log("tick");
        //     Debug.Log($"57: inputProvider.Touch1Position() >>>\n{inputProvider.Touch1Position()}");
        //     Debug.Log($"58: inputProvider.Touch2Position() >>>\n{inputProvider.Touch2Position()}");
        //     Debug.Log($"61: inputProvider.Touch0() >>>\n{inputProvider.Touch0()}");
        // }

        void Touch1Started(InputAction.CallbackContext ctx)
        {
            Debug.Log("Touch 1 started");
            Debug.Log($"53: inputProvider.Touch1Position() >>>\n{inputProvider.Touch1Position()}");
        }

        void Touch2Started(InputAction.CallbackContext ctx)
        {
            Debug.Log("Touch 2 started");
            Debug.Log($"58: inputProvider.Touch2Position() >>>\n{inputProvider.Touch2Position()}");
        }
    }
}