using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.enums;
using ReupVirtualTwin.modelInterfaces;
using System.Linq;

namespace ReupVirtualTwin.models
{
    public class ObjectTags : MonoBehaviour, IObjectTags 
    {
        [SerializeField]
        private List<EditionTag> tags = new List<EditionTag>();

        public List<EditionTag> GetTags()
        {
            return tags;
        }

        public List<EditionTag> AddTag(EditionTag tag)
        {
            tags.Add(tag);
            return tags;
        }

        public List<EditionTag> RemoveTag(EditionTag tag)
        {
            tags.Remove(tag);
            return tags;
        }

        public List<EditionTag> AddTags(EditionTag[] tagsList)
        {
            tags.AddRange(tagsList);
            return tags;
        }
    }
}
