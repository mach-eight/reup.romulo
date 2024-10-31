using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helpers;
using UnityEngine;

namespace ReupVirtualTwin.editor
{
    public static class TransferTagsUtils
    {
        static ITagSystemController tagSystemController = new TagSystemController();
        static ITagsController tagsController = new TagsController();

        public static void TransferTags(GameObject origin, GameObject target)
        {
            tagSystemController.AssignTagSystemToTree(target);
            tagsController.RemoveAllTagsFromTree(target);
            TransferTagsInTree(origin, target);
        }

        static void TransferTagsInTree(GameObject origin, GameObject target)
        {
            TransferTagsInObject(origin, target);
            List<GameObject> originChildren = origin.GetChildren();
            List<GameObject> targetChildren = target.GetChildren();
            Dictionary<string, GameObject> targetChildrenByNames = GameObjectUtils.MapGameObjectsByName(targetChildren);
            foreach (GameObject originChild in originChildren)
            {
                GameObject targetChild;
                if (targetChildrenByNames.TryGetValue(originChild.name, out targetChild))
                {
                    TransferTagsInTree(originChild, targetChild);
                }
            }
        }

        static void TransferTagsInObject(GameObject origin, GameObject target)
        {
            List<Tag> tags = tagsController.GetTagsFromObject(origin);
            tagsController.AddTagsToObject(target, tags);
        }

    }

}