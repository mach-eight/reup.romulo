using UnityEngine;
using ReupVirtualTwin.helpers;
using UnityEngine.InputSystem;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IRayProvider))]
    public abstract class Select : MonoBehaviour
    {
        protected InputProvider inputProvider;
        protected IRayProvider _rayProvider;
        protected IDragManager dragManager;

        protected virtual void Awake()
        {
            _rayProvider = GetComponent<IRayProvider>();
        }

        [Inject]
        public void Init(IDragManager dragManager, InputProvider inputProvider)
        {
            this.dragManager = dragManager;
            this.inputProvider = inputProvider;
        }

        private void OnEnable()
        {
            inputProvider.selectPerformed += OnSelect;
        }


        private void OnDisable()
        {
            inputProvider.selectPerformed -= OnSelect;
        }


        public abstract void OnSelect(InputAction.CallbackContext ctx);
    }
}