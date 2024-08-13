using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ReupVirtualTwin.helpers
{
    public static class TagFilterUtils
    {
        public delegate bool FilterFunction(GameObject gameObject);
        public static HashSet<GameObject> ExecuteFilterInTree(GameObject treeHead, FilterFunction filterFunction, bool invertedFilter)
        {
            HashSet<GameObject> filteredObjects = new HashSet<GameObject>();
            bool objectPassFilter = filterFunction(treeHead);
            if (objectPassFilter && invertedFilter)
            {
                return filteredObjects;
            }
            if (objectPassFilter && !invertedFilter)
            {
                filteredObjects.Add(treeHead);
                return filteredObjects;
            }
            List<GameObject> children = treeHead.GetChildren();
            children.ForEach(child =>
            {
                HashSet<GameObject> filteredFromChild = ExecuteFilterInTree(child, filterFunction, invertedFilter);
                filteredObjects.UnionWith(filteredFromChild);
            });
            if (invertedFilter && !objectPassFilter)
            {
                bool allChildrenPassed = children.All(child => filteredObjects.Contains(child)); 
                if (allChildrenPassed)
                {
                    filteredObjects.Add(treeHead);
                }
            }
            return filteredObjects;
        }
    }
}
