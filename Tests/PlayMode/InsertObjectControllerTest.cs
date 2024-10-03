using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Threading.Tasks;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using System.Collections;
using ReupVirtualTwin.helpers;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.models;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwinTests.controllers
{
    public class InsertObjectControllerTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        ObjectRegistry objectRegistryGameObject;
        MediatorSpy mediatorSpy;
        MeshDownloaderSpy meshDownloaderSpy;
        InsertObjectMessagePayload insertObjectMessagePayload;
        InsertObjectController controller;
        MockModelInfoManager mockModelInfoManager;
        ITagsController tagsReader;
        IIdGetterController idReader;
        Vector3 insertPosition;

        private void RequesObject(InsertObjectMessagePayload message)
        {
            mediatorSpy.IncreaseObjectRequestedCount();
            controller.InsertObject(message);
        }
        private bool HasObjectTreeColliders(GameObject obj)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Collider collider = obj.GetComponent<Collider>();
            if (meshFilter != null && meshFilter.sharedMesh != null && collider == null)
            {
                return false;
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (!HasObjectTreeColliders(obj.transform.GetChild(i).gameObject))
                {
                    return false;
                }
            }
            return true;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            objectRegistryGameObject = sceneObjects.objectRegistry;
            mediatorSpy = new MediatorSpy();
            meshDownloaderSpy = new MeshDownloaderSpy();
            mockModelInfoManager = new MockModelInfoManager();
            insertObjectMessagePayload = new InsertObjectMessagePayload()
            {
                objectId = "object-id",
                objectUrl = "object-url",
                selectObjectAfterInsertion = true,
                deselectPreviousSelection = true,
            };
            insertPosition = new Vector3(1, 2, 3);
            controller = new InsertObjectController(mediatorSpy, meshDownloaderSpy, insertPosition, mockModelInfoManager);
            RequesObject(insertObjectMessagePayload);
            tagsReader = new TagsController();
            idReader = new IdController();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldCreateInsertObjectController()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.IsNotNull(controller);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRequestMeshDownload()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.AreEqual(1, meshDownloaderSpy.numberOfCalls);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotifyProgressToMediator()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.AreEqual(4, mediatorSpy.onProgressNumberOfCalls);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotifyWhenObjectLoadsToMediator()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.AreEqual(meshDownloaderSpy.GetLastLoadedObject(), mediatorSpy.GetLastLoadedObject());
            Assert.IsTrue(mediatorSpy.GetLastLoadedObject().activeInHierarchy);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InsertedObjectShouldHaveSelectableTag()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.IsTrue(tagsReader.DoesObjectHaveTag(mediatorSpy.GetLastLoadedObject(), EditionTagsCreator.CreateSelectableTag().id));
            yield return null;
        }

        [UnityTest]
        public IEnumerator InsertedObjectShouldHaveTransformableTag()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.IsTrue(tagsReader.DoesObjectHaveTag(mediatorSpy.GetLastLoadedObject(), EditionTagsCreator.CreateTransformableTag().id));
            yield return null;
        }

        [UnityTest]
        public IEnumerator InsertedObjectShouldHaveDeletableTag()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.IsTrue(tagsReader.DoesObjectHaveTag(mediatorSpy.GetLastLoadedObject(), EditionTagsCreator.CreateDeletableTag().id));
            yield return null;
        }

        [UnityTest]
        public IEnumerator InsertedObjectShouldHaveCorrectPosition()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.AreEqual(insertPosition, mediatorSpy.GetLastLoadedObject().transform.position);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InsertedObjectShouldHaveDefinedId()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.AreEqual(insertObjectMessagePayload.objectId, idReader.GetIdFromObject(mediatorSpy.GetLastLoadedObject()));
            yield return null;
        }

        [UnityTest]
        public IEnumerator InsertedObjectShouldHaveColliders()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.IsTrue(HasObjectTreeColliders(mediatorSpy.GetLastLoadedObject()));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldPreserveDifferentSettingsForSimultaneouslyInsertedObjects()
        {
            InsertObjectMessagePayload anotherInsertMessagePayload = new()
            {
                objectId = "object-id-2",
                objectUrl = "object-url-2",
                selectObjectAfterInsertion = false,
            };
            RequesObject(anotherInsertMessagePayload);
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);

            InsertedObjectPayload firstObjectPayload = mediatorSpy.loadedObjectsPayloads.Find(payload => payload.selectObjectAfterInsertion == true);
            InsertedObjectPayload secondObjectPayload = mediatorSpy.loadedObjectsPayloads.Find(payload => payload.selectObjectAfterInsertion == false);

            Assert.AreEqual(meshDownloaderSpy.loadedObjects[0], firstObjectPayload.loadedObject);
            Assert.AreEqual(meshDownloaderSpy.loadedObjects[1], secondObjectPayload.loadedObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldInsertObjectToBuilding()
        {
            yield return new WaitUntil(() => mediatorSpy.allRequestedObjectsAreLoaded);
            Assert.AreEqual(1, mockModelInfoManager.objectsAddedToBuilding);
            yield return null;
        }

        private class MediatorSpy : IMediator
        {
            public int onProgressNumberOfCalls = 0;
            public List<InsertedObjectPayload> loadedObjectsPayloads;
            public bool allRequestedObjectsAreLoaded;

            private int requestedObjectsCount;
            private int loadedObjectsCount;
            private delegate void ObjectLoadedEventHandler();
            private event ObjectLoadedEventHandler ObjectLoaded;


            public MediatorSpy()
            {
                loadedObjectsPayloads = new List<InsertedObjectPayload>();
                requestedObjectsCount = 0;
                allRequestedObjectsAreLoaded = false;
                ObjectLoaded += () => NewLoadedObject();
            }
            public void IncreaseObjectRequestedCount()
            {
                requestedObjectsCount++;
            }
            public InsertedObjectPayload GetLastInsertedObjectPayload()
            {
                return loadedObjectsPayloads[loadedObjectsPayloads.Count - 1];
            }
            public GameObject GetLastLoadedObject()
            {
                return GetLastInsertedObjectPayload().loadedObject;
            }
            private void NewLoadedObject()
            {
                loadedObjectsCount++;
                if (loadedObjectsCount == requestedObjectsCount)
                {
                    allRequestedObjectsAreLoaded = true;
                    ObjectLoaded -= NewLoadedObject;
                }
            }

            public void Notify(ReupEvent eventName)
            {
                throw new System.NotImplementedException();
            }

            public void Notify<T>(ReupEvent eventName, T payload)
            {
                switch (eventName)
                {
                    case ReupEvent.insertedObjectStatusUpdate:
                        onProgressNumberOfCalls++;
                        break;
                    case ReupEvent.insertedObjectLoaded:
                        loadedObjectsPayloads.Add(((InsertedObjectPayload)(object)payload));
                        ObjectLoaded?.Invoke();
                        break;
                }
            }
        }

        private class MeshDownloaderSpy : IMeshDownloader
        {
            public int numberOfCalls;
            public List<GameObject> loadedObjects;

            public MeshDownloaderSpy()
            {
                loadedObjects = new List<GameObject>();
                numberOfCalls = 0;
            }
            public GameObject GetLastLoadedObject()
            {
                return loadedObjects[loadedObjects.Count - 1];
            }

            private GameObject CreateGameObject()
            {
                GameObject parent = new();
                GameObject child = new();
                MeshFilter meshFilter = child.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = new Mesh();
                child.transform.parent = parent.transform;
                return parent;
            }

            public async void downloadMesh(string meshUrl, Action<ModelLoaderContext, float> onProgress, Action<ModelLoaderContext> onLoad, Action<ModelLoaderContext> onMaterialsLoad)
            {
                numberOfCalls++;
                int downloadTime = 60;
                int processingTime = 30;
                GameObject obj = CreateGameObject();
                loadedObjects.Add(obj);
                ModelLoaderContext modelLoaderContext = new()
                {
                    loadedObject = obj,
                };
                onProgress(modelLoaderContext, 0.3f);
                onProgress(modelLoaderContext, 0.6f);
                onProgress(modelLoaderContext, 0.9f);
                onProgress(modelLoaderContext, 1f);
                await Task.Delay(downloadTime);
                onLoad(modelLoaderContext);
                await Task.Delay(processingTime);
                onMaterialsLoad(modelLoaderContext);
            }

            private class AssetLoaderContextStub
            {
                public GameObject RootGameObject;
            }
        }
        private class MockModelInfoManager : IModelInfoManager
        {
            public int objectsAddedToBuilding = 0;
            public MockModelInfoManager()
            {
                objectsAddedToBuilding = 0;
            }

            public WebMessage<JObject> ObtainModelInfoMessage()
            {
                throw new System.NotImplementedException();
            }

            public WebMessage<JObject> ObtainUpdateBuildingMessage()
            {
                throw new System.NotImplementedException();
            }

            public void InsertObjectToBuilding(GameObject obj)
            {
                objectsAddedToBuilding += 1;
            }

        }
    }
}
