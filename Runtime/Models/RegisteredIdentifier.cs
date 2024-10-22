using ReupVirtualTwin.helpers;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
    public class RegisteredIdentifier : UniqueId
    {
        public string manualId = "";
        public IObjectRegistry objectRegistry;

        public void Awake()
        {
            if (manualId != uniqueId && !string.IsNullOrEmpty(manualId))
            {
                uniqueId = manualId;
            }
            FindObjectRegistry();
            GenerateId();
        }

        override public string GenerateId()
        {

            if (objectRegistry == null)
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
            if (objectRegistry == null)
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
            objectRegistry.AddObject(gameObject);
        }

        private void UnRegisterObject()
        {
            string id = gameObject.GetComponent<IUniqueIdentifier>()?.getId();
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            GameObject registeredItem = objectRegistry.GetObjectWithGuid(id);
            if (registeredItem == gameObject)
            {
                objectRegistry.RemoveObject(id, gameObject);
            }
        }

        private void FindObjectRegistry()
        {
            if (objectRegistry != null)
            {
                return;
            }
            objectRegistry = ObjectFinder.FindObjectRegistry().GetComponent<IObjectRegistry>();
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
