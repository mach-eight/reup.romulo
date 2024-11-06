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