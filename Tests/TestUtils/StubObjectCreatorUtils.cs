using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEngine;

namespace ReupVirtualTwinTests.utils
{
    public static class StubObjectCreatorUtils
    {
        public static void AssignTagsToObject(GameObject obj, Tag[] tags)
        {
            ITagSystemController tagSystemController = new TagSystemController();
            tagSystemController.AssignTagSystemToObject(obj);
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            objectTags.AddTags(tags);
        }

        public static void AssignIdToObject(GameObject obj, string id)
        {
            IUniqueIdentifier identifier = obj.AddComponent<UniqueId>();
            identifier.AssignId(id);
        }
    }
}
