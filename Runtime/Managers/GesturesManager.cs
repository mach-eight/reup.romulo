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

        public event Action GestureStarted;
        public event Action GestureEnded;

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
            if (activeTouchInputs > 1 && !gestureInProgress)
            {
                gestureInProgress = true;
                GestureStarted?.Invoke();
            }
        }

        private void TouchStopped(InputAction.CallbackContext ctx)
        {
            activeTouchInputs--;
            if (activeTouchInputs < 2 && gestureInProgress)
            {
                gestureInProgress = false;
                GestureEnded?.Invoke();
            }
        }
    }
}