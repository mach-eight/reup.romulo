using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class GameObjectUtils
    {
        public static bool IsPartOf(List<GameObject> parents, GameObject child)
        {
            return parents.Exists(parent => IsPartOf(parent, child));
        }
        public static bool IsPartOf(GameObject parent, GameObject child)
        {
            if (child == null || parent == null)
            {
                return false;
            }
            Transform parentTransform = parent.transform;
            Transform childTransform = child.transform;
            while (childTransform != null)
            {
                if (childTransform == parentTransform)
                {
                    return true;
                }
                childTransform = childTransform.parent;
            }
            return false;
        }
        public static void ApplyLayerToObjectTree(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                ApplyLayerToObjectTree(child.gameObject, layer);
            }
        }
    }
}
