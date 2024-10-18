using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using ReupVirtualTwin.controllerInterfaces;
using System.Linq;

namespace ReupVirtualTwin.editor
{
    public static class ObjectVisibilityUtils
    {
        static SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;
        public static Dictionary<string, bool> GetVisibilityStateOfAllObjects(GameObject obj, IIdGetterController idGetter)
        {
            bool visibilityState = !visibilityManager.IsHidden(obj, false);
            string objId = idGetter.GetIdFromObject(obj);
            List<Dictionary<string, bool>> childrenStates = new List<Dictionary<string, bool>>();
            foreach (Transform child in obj.transform)
            {
                childrenStates.Add(GetVisibilityStateOfAllObjects(child.gameObject, idGetter));
            }
            Dictionary<string, bool> mergedStates = childrenStates.Aggregate(
                new Dictionary<string, bool>(),
                (acc, curr) => acc.Union(curr).ToDictionary(pair => pair.Key, pair => pair.Value)
            );
            mergedStates.Add(objId, visibilityState);
            return mergedStates;
        }

        public static void ApplyVisibilityState(GameObject obj, Dictionary<string, bool> visibilityStates, IIdGetterController idGetter)
        {
            string objId = idGetter.GetIdFromObject(obj);
            bool objectVisibility = visibilityStates.GetValueOrDefault(objId);
            ApplyVisibilityToObject(obj, objectVisibility);
            foreach (Transform child in obj.transform)
            {
                ApplyVisibilityState(child.gameObject, visibilityStates, idGetter);
            }
        }

        public static void ShowWholeObject(GameObject obj){
            visibilityManager.Show(obj, true);
        }

        private static void ApplyVisibilityToObject(GameObject obj, bool visibility)
        {
            if (visibility)
            {
                visibilityManager.Show(obj, false);
                return;
            }
            visibilityManager.Hide(obj, false);
        }
    }
}
