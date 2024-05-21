using ReupVirtualTwin.controllerInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.controllers
{
    public class SubstringTagFilter : ITagFilter
    {
        public bool invertFilter { get => _invertFilter; set => _invertFilter = value; }
        public string displayText => _displayText;
        public Action onRemoveFilter { set => _onRemoveFilter = value; }

        private bool _invertFilter = false;
        private string _displayText;
        private Action _onRemoveFilter;
        private ITagsController tagsController = new TagsController();

        public SubstringTagFilter(string tagSubstring)
        {
            _displayText = tagSubstring;
        }

        public HashSet<GameObject> ExecuteFilter(GameObject gameObject)
        {
            return TagFilterUtils.ExecuteFilter(gameObject, FilterFunction, _invertFilter);
        }

        public void RemoveFilter()
        {
            _onRemoveFilter?.Invoke();
        }

        private bool FilterFunction(GameObject gameObject)
        {
            string[] tagNames = tagsController.GetTagNamesFromObject(gameObject);
            return tagNames.Any(tagName => tagName.Contains(_displayText));
        }
    }
}
