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

namespace ReupVirtualTwinTests.controllers
{
    public class InsertObjectControllerTest
    {

        MediatorSpy mediatorSpy;
        MeshDownloaderSpy meshDownloaderSpy;
        InsertObjectMessagePayload insertObjectMessagePayload;
        InserObjectController controller;
        ITagsController tagsReader;

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
            controller = new InserObjectController(mediatorSpy, meshDownloaderSpy);
            controller.InsertObject(insertObjectMessagePayload);
            tagsReader = new TagsController();
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
            loadedObject = new GameObject();
            modelLoaderContext = new ModelLoaderContext()
            {
                loadedObject = loadedObject,
            };
        }
        //public void downloadMesh(string meshUrl, Action<T, float> onProgress, Action<T> onLoad, Action<T> onMaterialsLoad, Action<E> onError = null)
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

        //{
        //    throw new NotImplementedException();
        //}

        private class AssetLoaderContextStub
        {
            public GameObject RootGameObject;
        }
    }
}
