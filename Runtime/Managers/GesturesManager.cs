using ReupVirtualTwin.inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReupVirtualTwin.managers
{
    public class GesturesManager : MonoBehaviour
    {
        InputProvider _inputProvider;
        private int activeTouchInputs = 0;

        public bool gestureInProgress = false;

        private void Awake()
        {
            _inputProvider = new InputProvider();
        }

        private void OnEnable()
        {
            _inputProvider.firstTouchStarted += TouchStarted;
            _inputProvider.firstTouchCanceled += TouchStopped; 
            _inputProvider.secondTouchStarted += TouchStarted;
            _inputProvider.secondTouchCanceled += TouchStopped;          
        }

        private void OnDisable()
        {
            _inputProvider.firstTouchStarted -= TouchStarted;
            _inputProvider.firstTouchCanceled -= TouchStopped; 
            _inputProvider.secondTouchStarted -= TouchStarted;
            _inputProvider.secondTouchCanceled -= TouchStopped;        
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