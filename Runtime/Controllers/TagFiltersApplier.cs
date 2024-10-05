using ReupVirtualTwin.controllerInterfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public static class TagFiltersApplier
    {

        public static List<GameObject> ApplyFiltersToTree(GameObject obj, List<ITagFilter> filterList)
        {
            List<HashSet<GameObject>> filteredObjectsList = new List<HashSet<GameObject>>();
            List<ITagFilter> activeFilters = filterList.Where(filter => filter.filterIsActive).ToList();
            for (int i = 0; i < activeFilters.Count; i++)
            {
                filteredObjectsList.Add(filterList[i].ExecuteFilter(obj));
            }
            List<GameObject> objectsThatPassedAllFilters = filteredObjectsList.Skip(1)
            .Aggregate(
                new HashSet<GameObject>(filteredObjectsList.First()),
                (objectsIntersection, nextObjects) =>
                {
                    objectsIntersection.IntersectWith(nextObjects); return objectsIntersection;
                }
            ).ToList();
            return objectsThatPassedAllFilters;
        }

    }
}
