using NUnit.Framework;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ReupVirtualTwinTests.controllers
{
    public class InsertObjectControllerTest: MonoBehaviour
    {
        GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
        GameObject objectRegistryGameObject;
        MediatorSpy mediatorSpy;
        MeshDownloaderSpy meshDownloaderSpy;
        InsertObjectMessagePayload insertObjectMessagePayload;
        InserObjectController controller;
        ITagsController tagsReader;
        IIdGetterController idReader;
        Vector3 insertPosition;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // we set the objectRegistry only once because some objects that depend on it are use the ObjectFinder class to find it
            // if we create a different objectRegistry for each test in the SetUp method, the ObjectFinder sometimes finds
            // an old objectRegistry why this happens is still unknown to me
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
        }

        [SetUp]
        public void SetUp()
        {
            mediatorSpy = new MediatorSpy();
            meshDownloaderSpy = new MeshDownloaderSpy();
            insertObjectMessagePayload = new InsertObjectMessagePayload()
            {
                objectId = "object-id",
                objectUrl = "object-url",
                selectObjectAfterInsertion = true,
                deselectPreviousSelection = true,
            };
            insertPosition = new Vector3(1, 2, 3);
            controller = new InserObjectController(mediatorSpy, meshDownloaderSpy, insertPosition);
            controller.InsertObject(insertObjectMessagePayload);
            tagsReader = new TagsController();
            idReader = new IdController();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Destroy(objectRegistryGameObject);
        }

        [Test]
        public void ShouldCreateInsertObjectController()
        {
            Assert.IsNotNull(controller);
        }

        [Test]
        public void ShouldRequestMeshDownload()
        {
            Assert.AreEqual(1, meshDownloaderSpy.numberOfCalls);
            Assert.AreEqual(insertObjectMessagePayload.objectUrl, meshDownloaderSpy.meshUrl);
        }

        [Test]
        public void ShouldNotifyMediatorForProgress()
        {
            Assert.AreEqual(4, mediatorSpy.onProgressNumberOfCalls);
            Assert.AreEqual(new List<float> { 0.3f, 0.6f, 0.9f, 1f }, mediatorSpy.progresses);
        }

        [Test]
        public void ShouldNotifyMediatorForLoad()
        {
            Assert.AreEqual(meshDownloaderSpy.loadedObject, mediatorSpy.loadedObject);
            Assert.IsTrue(mediatorSpy.loadedObject.activeInHierarchy);
        }

        [Test]
        public void InsertedObjectShouldHaveSelectableTag()
        {
            Assert.IsTrue(tagsReader.DoesObjectHaveTag(mediatorSpy.loadedObject, ObjectTag.SELECTABLE));
        }

        [Test]
        public void InsertedObjectShouldHaveTransformableTag()
        {
            Assert.IsTrue(tagsReader.DoesObjectHaveTag(mediatorSpy.loadedObject, ObjectTag.TRANSFORMABLE));
        }

        [Test]
        public void InsertedObjectShouldHaveDeletableTag()
        {
            Assert.IsTrue(tagsReader.DoesObjectHaveTag(mediatorSpy.loadedObject, ObjectTag.DELETABLE));
        }

        [Test]
        public void InsertedObjectShouldHaveCorrectPosition()
        {
            Assert.AreEqual(insertPosition, mediatorSpy.loadedObject.transform.position);
        }

        [Test]
        public void InsertedObjectShouldHaveDefinedId()
        {
            Assert.AreEqual(insertObjectMessagePayload.objectId, idReader.GetIdFromObject(mediatorSpy.loadedObject));
        }

        [Test]
        public void InsertedObjectShouldHaveColliders()
        {
            Assert.IsTrue(CheckMeshesCollider(mediatorSpy.loadedObject));
        }

        private bool CheckMeshesCollider(GameObject obj)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Collider collider = obj.GetComponent<Collider>();
            if (meshFilter != null && meshFilter.sharedMesh != null && collider == null)
            {
                return false;
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (!CheckMeshesCollider(obj.transform.GetChild(i).gameObject))
                {
                    return false;
                }
            }
            return true;
        }

    }

    class MediatorSpy : IMediator
    {
        public int onProgressNumberOfCalls = 0;
        public List<float> progresses = new();
        public GameObject loadedObject = null;
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
                    progresses.Add((float)(object)payload);
                    break;
                case ReupEvent.insertedObjectLoaded:
                    loadedObject = ((InsertedObjectPayload)(object)payload).loadedObject;
                    break;
            }
        }
    }

    class MeshDownloaderSpy : IMeshDownloader
    {
        public string meshUrl;
        public int numberOfCalls;
        public GameObject loadedObject;
        private ModelLoaderContext modelLoaderContext;
        public MeshDownloaderSpy()
        {
            numberOfCalls = 0;
            loadedObject = CreateGameObject();
            modelLoaderContext = new ModelLoaderContext()
            {
                loadedObject = loadedObject,
            };
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

        public void downloadMesh(string meshUrl, Action<ModelLoaderContext, float> onProgress, Action<ModelLoaderContext> onLoad, Action<ModelLoaderContext> onMaterialsLoad)
        {
            numberOfCalls++;
            this.meshUrl = meshUrl;
            onProgress(modelLoaderContext, 0.3f);
            onProgress(modelLoaderContext, 0.6f);
            onProgress(modelLoaderContext, 0.9f);
            onProgress(modelLoaderContext, 1f);
            onLoad(modelLoaderContext);
            onMaterialsLoad(modelLoaderContext);
        }

        private class AssetLoaderContextStub
        {
            public GameObject RootGameObject;
        }
    }
}
