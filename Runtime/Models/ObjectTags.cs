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
        private List<string> tags = new List<string>();

        public List<string> GetTags()
        {
            return tags;
        }

        public List<string> AddTag(string tag)
        {
            tags.Add(tag);
            return tags;
        }

        public List<string> RemoveTag(string tag)
        {
            tags.Remove(tag);
            return tags;
        }

        public List<string> AddTags(string[] tagsList)
        {
            tags.AddRange(tagsList);
            return tags;
        }
    }
}
