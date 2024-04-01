using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.controllers
{
    public static class FinderController
    {
        public static ITagsApiManager FindTagsApiManager()
        {
            ITagsApiManager tagsApiManager = GameObject.FindGameObjectWithTag(TagsEnum.tagsApiManager).GetComponent<ITagsApiManager>();
            if (tagsApiManager.webRequester == null)
            {
                tagsApiManager.webRequester = new TagsWebRequesterController("http://localhost:8000/api/v1/");
                //tagsApiManager.webRequester = new TagsWebRequesterController("https://api-prod-reup.macheight.com/api/v1/");
            }
            return tagsApiManager;
        }
    }
}
