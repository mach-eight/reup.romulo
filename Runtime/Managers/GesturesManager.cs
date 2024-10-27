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
        }

        public void Dispose()
        {
            inputProvider.touch1HoldStarted -= TouchStarted;
            inputProvider.touch1HoldSCanceled -= TouchStopped;
            inputProvider.touch2HoldStarted -= TouchStarted;
            inputProvider.touch2HoldCanceled -= TouchStopped;
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
    }
}