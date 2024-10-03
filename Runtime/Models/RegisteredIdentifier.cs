using ReupVirtualTwin.helpers;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
    public class RegisteredIdentifier : UniqueId
    {
        public string manualId = "";
        private IObjectRegistry _objectRegistry;

        override protected void Start()
        {
            if (manualId != uniqueId && !string.IsNullOrEmpty(manualId))
            {
                uniqueId = manualId;
            }
            FindObjectRegistry();
            base.Start();
        }

        override public string GenerateId()
        {

            if (_objectRegistry == null)
            {
                FindObjectRegistry();
            }
            UnRegisterObject();
            base.GenerateId();
            RegisterObject();
            return uniqueId;
        }

        public override string AssignId(string id)
        {
            if (_objectRegistry == null)
            {
                FindObjectRegistry();
            }
            UnRegisterObject();
            base.AssignId(id);
            RegisterObject();
            return uniqueId;
        }
        private void RegisterObject()
        {
            _objectRegistry.AddObject(gameObject);
        }

        private void UnRegisterObject()
        {
            string id = gameObject.GetComponent<IUniqueIdentifier>()?.getId();
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            GameObject registeredItem = _objectRegistry.GetObjectWithGuid(id);
            if (registeredItem == gameObject)
            {
                _objectRegistry.RemoveObject(id, gameObject);
            }
        }

        private void FindObjectRegistry()
        {
            _objectRegistry = ObjectFinder.FindObjectRegistry().GetComponent<IObjectRegistry>();
        }

        public override string getId()
        {
            if (string.IsNullOrEmpty(manualId))
            {
                return base.getId();
            }
            return manualId;
        }

    }
}
