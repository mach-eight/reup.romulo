using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface ITagFilter
    {
        public bool filterIsActive { get; set; }
        public bool invertFilter { get; set; }
        public string displayText { get; }
        public Action onRemoveFilter { set; }
        public HashSet<GameObject> ExecuteFilter(GameObject gameObject);
        public void RemoveFilter();
    }
}
