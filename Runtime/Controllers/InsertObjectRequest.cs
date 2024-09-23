using UnityEngine;

using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.controllers
{
    public class InsertObjectRequest
    {
        private IMediator mediator;
        private IModelInfoManager modelInfoManager;
        private InsertObjectMessagePayload insertObjectMessagePayload;
        private ITagSystemController tagSystemController = new TagSystemController();
        private IIdAssignerController idAssigner = new IdController();
        private IColliderAdder colliderAdder = new  ColliderAdder();
        private Vector3 insertPosition;
        public InsertObjectRequest(IMediator mediator, IMeshDownloader meshDownloader, InsertObjectMessagePayload messagePayload, Vector3 insertPosition, IModelInfoManager modelInfoManager)
        {
            this.insertPosition = insertPosition;
            this.mediator = mediator;
            this.modelInfoManager = modelInfoManager;
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

        private void OnLoad(ModelLoaderContext assetLoaderContext)
        {
            GameObject loadedObj = assetLoaderContext.loadedObject;
            AddTags(loadedObj);
            SetLoadPosition(loadedObj);
            AddColliders(loadedObj);
            AssignIds(loadedObj);
            InsertToBuilding(loadedObj);
            loadedObj.SetActive(false);
        }
        private void AddTags(GameObject obj)
        {
            IObjectTags objectTags = tagSystemController.AssignTagSystemToObject(obj);
            objectTags.AddTags(SystemTagsCreator.CreateSystemTags().ToArray());
        }
        private void SetLoadPosition(GameObject obj)
        {
            obj.transform.position = insertPosition;
        }
        private void AddColliders(GameObject obj)
        {
            colliderAdder.AddCollidersToTree(obj);
        }
        private void AssignIds(GameObject obj)
        {
            idAssigner.AssignIdsToTree(obj, insertObjectMessagePayload.objectId);
        }

        private void InsertToBuilding(GameObject obj)
        {
            modelInfoManager.InsertObjectToBuilding(obj);
        }

        private void OnMaterialsLoad(ModelLoaderContext assetLoaderContext)
        {
            var obj = assetLoaderContext.loadedObject;
            obj.SetActive(true);
            InsertedObjectPayload insertedObjectPayload = new()
            {
                loadedObject = obj,
                selectObjectAfterInsertion = insertObjectMessagePayload.selectObjectAfterInsertion,
            };
            mediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        }

    }
}
