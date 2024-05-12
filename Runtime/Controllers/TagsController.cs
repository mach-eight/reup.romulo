using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.controllers
{
    public class TagsController : ITagsController
    {
        public List<Tag> AddTagToObject(GameObject obj, Tag tag)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags == null)
            {
                throw new InvalidOperationException("Can not add tag. Tag system is not attached to object");
            }
            return objectTags.AddTag(tag);
        }

        public bool DoesObjectHaveTag(GameObject obj, string tagId)
        {
            List<Tag> tags = GetTagsFromObject(obj);
            if (tags == null) return false;
            return tags.Select(tag => tag.id).Contains(tagId);
        }


        public string[] GetTagNamesFromObject(GameObject obj)
        {
            List<Tag> tags = GetTagsFromObject(obj);
            if (tags == null) return new string[0] { };
            return tags.Select(tag => tag.name).ToArray();
        }

        public List<Tag> GetTagsFromObject(GameObject obj)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags == null) return new List<Tag>() { };
            return objectTags.GetTags();
        }

        public List<Tag> RemoveTagFromObject(GameObject obj, Tag tag)
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
