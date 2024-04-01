using ReupVirtualTwin.enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface ITagsController
    {
        public List<string> GetTagsFromObject(GameObject obj);
        public List<string> AddTagToObject(GameObject obj, string tag);
        public List<string> RemoveTagFromOjbect(GameObject obj, string tag);
        public bool DoesObjectHaveTag(GameObject obj, string tag);
        public string[] GetTagNamesFromObject(GameObject obj);
    }
}
