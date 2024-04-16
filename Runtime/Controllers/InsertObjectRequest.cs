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
        private InsertObjectMessagePayload insertObjectMessagePayload;
        private ITagSystemController tagSystemController = new TagSystemController();
        private IIdAssignerController idAssigner = new IdController();
        private IColliderAdder colliderAdder = new  ColliderAdder();
        private Vector3 insertPosition;
        public InsertObjectRequest(IMediator mediator, IMeshDownloader meshDownloader, InsertObjectMessagePayload messagePayload, Vector3 insertPosition)
        {
            this.insertPosition = insertPosition;
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

        private void OnLoad(ModelLoaderContext assetLoaderContext)
        {
            GameObject loadedObj = assetLoaderContext.loadedObject;
            AddTags(loadedObj);
            SetLoadPosition(loadedObj);
            AddColliders(loadedObj);
            AssignIds(loadedObj);
            loadedObj.SetActive(false);
        }
        private GameObject AddTags(GameObject obj)
        {
            IObjectTags objectTags = tagSystemController.AssignTagSystemToObject(obj);
            objectTags.AddTags(new string[3] {
                EditionTag.DELETABLE.ToString(),
                EditionTag.SELECTABLE.ToString(),
                EditionTag.TRANSFORMABLE.ToString(),
            });
            return obj;
        }
        private GameObject SetLoadPosition(GameObject obj)
        {
            obj.transform.position = insertPosition;
            return obj;
        }
        private GameObject AddColliders(GameObject obj)
        {
            colliderAdder.AddCollidersToTree(obj);
            return obj;
        }
        private GameObject AssignIds(GameObject obj)
        {
            idAssigner.AssignIdsToTree(obj, insertObjectMessagePayload.objectId);
            return obj;
        }

        private void OnMaterialsLoad(ModelLoaderContext assetLoaderContext)
        {
            var obj = assetLoaderContext.loadedObject;
            obj.SetActive(true);
            InsertedObjectPayload insertedObjectPayload = new()
            {
                loadedObject = obj,
                selectObjectAfterInsertion = insertObjectMessagePayload.selectObjectAfterInsertion,
                deselectPreviousSelection = insertObjectMessagePayload.deselectPreviousSelection,
            };
            mediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        }

    }
}
