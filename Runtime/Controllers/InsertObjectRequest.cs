using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore;
using System;

namespace ReupVirtualTwin.controllers
{
    public class InsertObjectRequest
    {
        public InsertObjectRequest(IMediator mediator, IMeshDownloader meshDownloader, InsertObjectMessagePayload messagePayload)
        {
            meshDownloader.downloadMesh<AssetLoaderContext, IContextualizedError>(
                messagePayload.objectUrl,
                OnProgress,
                OnLoad,
                OnMaterialsLoad,
                OnError
            );
        }

        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            //_mediator.Notify(ReupEvent.insertedObjectStatusUpdate, progress);
        }

        private void OnError(IContextualizedError contextualizedError)
        {
            //Debug.LogError(contextualizedError);
        }

        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            //GameObject loadedObj = assetLoaderContext.RootGameObject;
            //AddTags(loadedObj);
            //SetLoadPosition(loadedObj);
            //AddColliders(loadedObj);
            //AssignIds(loadedObj);
            //loadedObj.SetActive(false);
        }
        private GameObject AddTags(GameObject obj)
        {
            //IObjectTags objectTags = _tagSystemController.AssignTagSystemToObject(obj);
            //objectTags.AddTags(new ObjectTag[3] {
            //    ObjectTag.SELECTABLE,
            //    ObjectTag.DELETABLE,
            //    ObjectTag.TRANSFORMABLE,
            //});
            return obj;
        }
        private GameObject SetLoadPosition(GameObject obj)
        {
            //obj.transform.position = _insertPositionLocation.transform.position;
            return obj;
        }
        private GameObject AddColliders(GameObject obj)
        {
            //_colliderAdder.AddCollidersToTree(obj);
            return obj;
        }
        private GameObject AssignIds(GameObject obj)
        {
            //_idAssigner.AssignIdsToTree(obj, objectId);
            return obj;
        }

        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            //var myLoadedGameObject = assetLoaderContext.RootGameObject;
            //myLoadedGameObject.SetActive(true);
            //_mediator.Notify(ReupEvent.insertedObjectLoaded, myLoadedGameObject);
        }

    }
}
