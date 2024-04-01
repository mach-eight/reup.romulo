using ReupVirtualTwin.enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface ITagsController
    {
        public List<EditionTag> GetTagsFromObject(GameObject obj);
        public List<EditionTag> AddTagToObject(GameObject obj, EditionTag tag);
        public List<EditionTag> RemoveTagFromOjbect(GameObject obj, EditionTag tag);
        public Boolean DoesObjectHaveTag(GameObject obj, EditionTag tag);
        public string[] GetTagNamesFromObject(GameObject obj);
    }
}
