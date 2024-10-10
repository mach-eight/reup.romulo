using ReupVirtualTwin.controllerInterfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public static class TagFiltersApplier
    {

        public static List<GameObject> ApplyExclusiveFiltersToTree(GameObject obj, List<ITagFilter> filterList)
        {
            List<ITagFilter> activeFilters = GetActiveFilters(filterList);
            if (activeFilters.Count == 0) return new List<GameObject>();
            List<HashSet<GameObject>> filteredObjectsList = GetFilteredObjectsList(obj, activeFilters);
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

        public static List<GameObject> ApplyInclusiveFiltersToTree(GameObject obj, List<ITagFilter> filterList)
        {
            List<ITagFilter> activeFilters = GetActiveFilters(filterList);
            if (activeFilters.Count == 0) return new List<GameObject>();
            List<HashSet<GameObject>> filteredObjectsList = GetFilteredObjectsList(obj, activeFilters);
            List<GameObject> objectsThatPassedAllFilters = filteredObjectsList.Skip(1)
            .Aggregate(
                new HashSet<GameObject>(filteredObjectsList.First()),
                (objectsUnion, nextObjects) =>
                {
                    objectsUnion.UnionWith(nextObjects); return objectsUnion;
                }
            ).ToList();
            return objectsThatPassedAllFilters;

        }

        static List<ITagFilter> GetActiveFilters(List<ITagFilter> filterList)
        {
            return filterList.Where(filter => filter.filterIsActive).ToList();
        }

        static List<HashSet<GameObject>> GetFilteredObjectsList(GameObject obj, List<ITagFilter> filterList)
        {
            return filterList.Select(filter => filter.ExecuteFilter(obj)).ToList();
        }

    }
}
