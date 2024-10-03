using UnityEngine;
using UnityEngine.InputSystem;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IRayCastHitSelector))]
    public abstract class SelectPoint : Select
    {
        private IRayCastHitSelector _hitSelector;


        protected override void Start()
        {
            base.Start();
            _hitSelector = GetComponent<IRayCastHitSelector>();
        }

        protected override void Awake()
        {
            base.Awake();
            _hitSelector = GetComponent<IRayCastHitSelector>();
        }

        public override void OnSelect(InputAction.CallbackContext ctx)
        {
            if (!_dragManager.prevDragging)
            {
                Ray ray = _rayProvider.GetRay();
                RaycastHit? hit = _hitSelector.GetHit(ray);
                if (hit != null)
                {
                    HandleHit((RaycastHit)hit);
                    return;
                }
            }
            MissHit();
        }
        public abstract void HandleHit(RaycastHit hit);
        public virtual void MissHit() { }
    }
}