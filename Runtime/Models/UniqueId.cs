using System;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
	public class UniqueIdentifierAttribute : PropertyAttribute { }

	public class UniqueId : MonoBehaviour, IUniqueIdentifier
	{
		[UniqueIdentifier]
		public string uniqueId;

        protected virtual void Start()
        {
            GenerateId();
        }

        virtual public string GenerateId()
        {
            if (uniqueId == null || uniqueId == "")
            {
                Guid guid = Guid.NewGuid();
                uniqueId = guid.ToString();
            }
            return uniqueId;
        }

        virtual public string AssignId(string id)
        {
            if (id == null || id == "")
            {
                throw new Exception("Id cannot be empty");
            }
            uniqueId = id;
            return uniqueId;
        }

        public string getId()
        {
            return uniqueId;
        }

        public bool isIdCorrect(string id)
        {
            return id == uniqueId;
        }
    }
}
