using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using UnityEngine;

namespace ReupVirtualTwin.editor
{
    public static class TagTransferUtils
    {
        static ITagSystemController tagSystemController = new TagSystemController();
        static ITagsController tagsController = new TagsController();

        public static void TransferTags(GameObject origin, GameObject target)
        {
            tagSystemController.AssignTagSystemToTree(target);
            tagsController.RemoveAllTagsFromTree(target);
        }

    }

}