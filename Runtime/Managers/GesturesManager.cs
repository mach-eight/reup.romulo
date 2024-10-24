using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ReupVirtualTwin.managers
{
    public class GesturesManager : MonoBehaviour, IGesturesManager
    {
        [HideInInspector] public bool gestureInProgress { get; private set; } = false;
        InputProvider _inputProvider;
        private int activeTouchInputs = 0;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            Debug.Log("in gestures manager init");
            this._inputProvider = inputProvider;
            Debug.Log($"19: this._inputProvider >>>\n{this._inputProvider}");
        }

        // void Awake()
        // {
        //     _inputProvider = new InputProvider();
        //     Debug.Log($"24: _inputProvider >>>\n{_inputProvider}");
        // }

        private void OnEnable()
        {
            Debug.Log("gestures manager enabled");
            _inputProvider.touch1HoldStarted += TouchStarted;
            _inputProvider.touch1HoldSCanceled += TouchStopped;
            _inputProvider.touch2HoldStarted += TouchStarted;
            _inputProvider.touch2HoldCanceled += TouchStopped;
        }

        private void OnDisable()
        {
            Debug.Log("gestures manager disabled");
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