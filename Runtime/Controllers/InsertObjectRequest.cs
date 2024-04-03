using UnityEngine;

using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class InsertObjectRequest
    {
        private IMediator mediator;
        private InsertObjectMessagePayload insertObjectMessagePayload;
        private ITagSystemController tagSystemController = new TagSystemController();
        public InsertObjectRequest(IMediator mediator, IMeshDownloader meshDownloader, InsertObjectMessagePayload messagePayload)
        {
            this.mediator = mediator;
            insertObjectMessagePayload = messagePayload;
            meshDownloader.downloadMesh(
                messagePayload.objectUrl,
                OnProgress,
                OnLoad,
                OnMaterialsLoad
            );
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

            AddTags(loadedObj);
            //SetLoadPosition(loadedObj);
            //AddColliders(loadedObj);
            //AssignIds(loadedObj);
            //loadedObj.SetActive(false);
        }
        private GameObject AddTags(GameObject obj)
        {
            IObjectTags objectTags = tagSystemController.AssignTagSystemToObject(obj);
            objectTags.AddTags(new ObjectTag[3] {
                ObjectTag.SELECTABLE,
                ObjectTag.DELETABLE,
                ObjectTag.TRANSFORMABLE,
            });
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
            var myLoadedGameObject = assetLoaderContext.loadedObject;
            myLoadedGameObject.SetActive(true);
            InsertedObjectPayload insertedObjectPayload = new()
            {
                loadedObject = myLoadedGameObject,
                selectObjectAfterInsertion = insertObjectMessagePayload.selectObjectAfterInsertion,
                deselectPreviousSelection = insertObjectMessagePayload.deselectPreviousSelection,
            };
            mediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        }

    }
}
