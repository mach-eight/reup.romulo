using UnityEngine;
using ReupVirtualTwin.helpers;
using UnityEngine.InputSystem;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IRayProvider))]
    public abstract class Select : MonoBehaviour
    {
        protected InputProvider _inputProvider;
        protected IRayProvider _rayProvider;
        protected IDragManager _dragManager;

        protected virtual void Awake()
        {
            _inputProvider = new InputProvider();
            _rayProvider = GetComponent<IRayProvider>();
        }
        protected virtual void Start()
        {
            _dragManager = ObjectFinder.FindDragManager().GetComponent<IDragManager>();
        }

        private void OnEnable()
        {
            _inputProvider.selectPerformed += OnSelect;
        }


        private void OnDisable()
        {
            _inputProvider.selectPerformed -= OnSelect;
        }


        public abstract void OnSelect(InputAction.CallbackContext ctx);
    }
}