using ReupVirtualTwin.models;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using System.Collections.Generic;

namespace ReupVirtualTwin.controllers
{
    public class IdController : IIdGetterController, IIdAssignerController, IIdHasRepeatedController
    {

        public void AssignIdsToTree(GameObject tree, string parentTreeId = null)
        {
            AssignIdToObject(tree, parentTreeId);
            foreach (Transform child in tree.transform)
            {
                AssignIdsToSubTree(child.gameObject);
            }
        }

        private void AssignIdsToSubTree(GameObject tree)
        {
            AssignIdToObject(tree);
            foreach (Transform child in tree.transform)
            {
                AssignIdsToSubTree(child.gameObject);
            }
        }

        public IUniqueIdentifier AssignIdToObject(GameObject obj, string objectId = null)
        {
            IUniqueIdentifier uniqueId = obj.GetComponent<IUniqueIdentifier>();
            if (uniqueId == null)
            {
                uniqueId = obj.AddComponent<RegisteredIdentifier>();
                CreateIdInUniqueIdComponent(uniqueId, objectId);
            }
            return uniqueId;
        }
        private void CreateIdInUniqueIdComponent(IUniqueIdentifier uniqueId, string idToAssign)
        {
            if (idToAssign != null)
            {
                uniqueId.AssignId(idToAssign);
                return;
            }
            uniqueId.GenerateId();
        }

        public void RemoveIdsFromTree(GameObject tree)
        {
            RemoveIdFromObject(tree);
            foreach (Transform child in tree.transform)
            {
                RemoveIdsFromTree(child.gameObject);
            }
        }

        public void RemoveIdFromObject(GameObject obj)
        {
            IUniqueIdentifier identifier = obj.GetComponent<IUniqueIdentifier>();
            if (identifier != null)
            {
                Object.DestroyImmediate((Object)identifier);
            }
        }

        public string GetIdFromObject(GameObject obj)
        {
            return obj.GetComponent<IUniqueIdentifier>().getId();
        }

        public bool HasRepeatedIds(GameObject tree)
        {
            HashSet<string> idSet = new HashSet<string>();
            return CheckForRepeatedIds(tree, idSet);
        }

        private bool CheckForRepeatedIds(GameObject tree, HashSet<string> idSet)
        {
            IUniqueIdentifier identifier = tree.GetComponent<IUniqueIdentifier>();
            if (identifier != null)
            {
                string id = identifier.getId();
                if (idSet.Contains(id))
                {
                    return true;
                }
                idSet.Add(id);
            }

            foreach (Transform child in tree.transform)
            {
                if (CheckForRepeatedIds(child.gameObject, idSet))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
