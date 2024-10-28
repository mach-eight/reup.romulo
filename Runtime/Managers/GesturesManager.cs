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
            inputProvider.touch1HoldStarted += TouchStarted;
            inputProvider.touch1HoldSCanceled += TouchStopped;
            inputProvider.touch2HoldStarted += TouchStarted;
            inputProvider.touch2HoldCanceled += TouchStopped;
            // inputProvider.touch1HoldStarted += Touch1Started;
            // inputProvider.touch2HoldStarted += Touch2Started;
            // inputProvider.touch0Started += Touch1Started;
        }

        public void Dispose()
        {
            inputProvider.touch1HoldStarted -= TouchStarted;
            inputProvider.touch1HoldSCanceled -= TouchStopped;
            inputProvider.touch2HoldStarted -= TouchStarted;
            inputProvider.touch2HoldCanceled -= TouchStopped;
            // inputProvider.touch1HoldStarted -= Touch1Started;
            // inputProvider.touch2HoldStarted -= Touch2Started;
            // inputProvider.touch0Started -= Touch1Started;
        }

        private void TouchStarted(InputAction.CallbackContext ctx)
        {
            activeTouchInputs++;
            gestureInProgress = activeTouchInputs > 1;
        }

        private void TouchStopped(InputAction.CallbackContext ctx)
        {
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