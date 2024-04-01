using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ReupVirtualTwin.enums;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class TagsController : ITagsController
    {
        public List<EditionTag> AddTagToObject(GameObject obj, EditionTag tag)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags == null)
            {
                throw new InvalidOperationException("Can not add tag. Tag system is not attached to object");
            }
            return objectTags.AddTag(tag);
        }

        public bool DoesObjectHaveTag(GameObject obj, EditionTag tag)
        {
            List<EditionTag> tags = GetTagsFromObject(obj);
            if (tags == null) return false;
            return tags.Contains(tag);
        }

        public string[] GetTagNamesFromObject(GameObject obj)
        {
            List<EditionTag> tags = GetTagsFromObject(obj);
            if (tags == null) return new string[0] { };
            return tags.Select(tag => tag.ToString()).ToArray();
        }

        public List<EditionTag> GetTagsFromObject(GameObject obj)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags == null) return new List<EditionTag>() { };
            return objectTags.GetTags();
        }

        public List<EditionTag> RemoveTagFromOjbect(GameObject obj, EditionTag tag)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags == null)
            {
                throw new InvalidOperationException("Can not remove tag. Tag system is not attached to object");
            }
            return objectTags.RemoveTag(tag);
        }
    }
}
