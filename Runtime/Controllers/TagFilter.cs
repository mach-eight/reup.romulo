using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.controllers
{
    public class TagFilter : ITagFilter
    {
        public bool invertFilter { get => _invertFilter; set => _invertFilter = value; }
        public string displayText => _displayText;
        public Action onRemoveFilter { set => _onRemoveFilter = value; }

        private bool _invertFilter = false;
        private string _displayText;
        private ITagsController tagsController = new TagsController();
        private Tag tag;
        private Action _onRemoveFilter;

        public TagFilter(Tag tag)
        {
            this.tag = tag;
            _displayText = tag.str;
        }

        private bool FilterFunction(GameObject gameObject)
        {
            return tagsController.DoesObjectHaveTag(gameObject, tag.id);
        }

        public HashSet<GameObject> ExecuteFilter(GameObject gameObject)
        {
            return TagFilterUtils.ExecuteFilterInTree(gameObject, FilterFunction, _invertFilter);
        }

        public void RemoveFilter()
        {
            _onRemoveFilter?.Invoke();
        }
    }
}
