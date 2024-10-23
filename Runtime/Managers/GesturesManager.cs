using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReupVirtualTwin.managers
{
    public class GesturesManager : MonoBehaviour, IGesturesManager
    {
        [HideInInspector] public bool gestureInProgress { get; private set; } = false;
        InputProvider _inputProvider;
        private int activeTouchInputs = 0;

        private void Awake()
        {
            _inputProvider = new InputProvider();
        }

        private void OnEnable()
        {
            _inputProvider.touch1HoldStarted += TouchStarted;
            _inputProvider.touch1HoldSCanceled += TouchStopped; 
            _inputProvider.touch2HoldStarted += TouchStarted;
            _inputProvider.touch2HoldCanceled += TouchStopped;          
        }

        private void OnDisable()
        {
            _inputProvider.touch1HoldStarted -= TouchStarted;
            _inputProvider.touch1HoldSCanceled -= TouchStopped; 
            _inputProvider.touch2HoldStarted -= TouchStarted;
            _inputProvider.touch2HoldCanceled -= TouchStopped;        
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