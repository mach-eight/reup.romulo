using ReupVirtualTwin.helpers;
using UnityEngine;
using UnityEngine.InputSystem;



namespace ReupVirtualTwin.behaviours
{

    [RequireComponent(typeof(IObjectSelector))]
    public abstract class SelectObject : Select
    {
        private IObjectSelector _objectSelector;

        protected override void Start()
        {
            base.Start();
            _objectSelector = GetComponent<IObjectSelector>();
        }

        public override void OnSelect(InputAction.CallbackContext ctx)
        {
            if (!_dragManager.prevDragging && !OverUICheck.PointerOverUI() && !_dragManager.prevSelectInputInUI)
            {
                Ray ray = _rayProvider.GetRay();
                //Debug.DrawRay(ray.origin, ray.direction, Color.red, 1);
                GameObject obj = _objectSelector.GetObject(ray);
                if (obj != null)
                {
                    HandleObject(obj);
                    return;
                }
            }
            MissObject();
        }
        public virtual void HandleObject(GameObject obj) { }
        public virtual void MissObject() { }
    }
}