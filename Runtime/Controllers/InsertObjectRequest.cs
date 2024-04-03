using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore;
using System;

using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class InsertObjectRequest
    {
        private IMediator mediator;
        public InsertObjectRequest(IMediator mediator, IMeshDownloader meshDownloader, InsertObjectMessagePayload messagePayload)
        {
            Debug.Log("InsertObjectRequest created");
            this.mediator = mediator;
            meshDownloader.downloadMesh(
            //meshDownloader.downloadMesh(
                messagePayload.objectUrl,
                OnProgress,
                OnLoad,
                OnMaterialsLoad
                //OnError
            );
            Debug.Log("downloadMesh called");
        }

        private void OnProgressw(ModelLoaderContext assetLoaderContext, float progress)
        {

        }
        private void OnProgress(ModelLoaderContext assetLoaderContext, float progress)
        {
            mediator.Notify(ReupEvent.insertedObjectStatusUpdate, progress);
        }

        //private void OnError(IContextualizedError contextualizedError)
        //{
        //    //Debug.LogError(contextualizedError);
        //}

        //private void OnLoad(AssetLoaderContext assetLoaderContext)
        private void OnLoad(ModelLoaderContext assetLoaderContext)
        {

            GameObject loadedObj = assetLoaderContext.loadedObject;
            Debug.Log("loaded game object: " + loadedObj.name);

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

        //private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        private void OnMaterialsLoad(ModelLoaderContext assetLoaderContext)
        {
            //var myLoadedGameObject = assetLoaderContext.RootGameObject;
            //myLoadedGameObject.SetActive(true);
            //_mediator.Notify(ReupEvent.insertedObjectLoaded, myLoadedGameObject);
        }

    }
}
