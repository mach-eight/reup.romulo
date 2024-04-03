using NUnit.Framework;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using System;
using System.Collections.Generic;
using TriLibCore;
using UnityEngine;

namespace ReupVirtualTwinTests.controllers
{
    public class InsertObjectControllerTest
    {

        MediatorSpy mediatorSpy;
        MeshDownloaderSpy meshDownloaderSpy;
        InsertObjectMessagePayload insertObjectMessagePayload;
        InserObjectController controller;

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

    }

    class MediatorSpy : IMediator
    {
        public int onProgressNumberOfCalls = 0;
        public List<float> progresses = new();
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
            }
        }
    }

    class MeshDownloaderSpy : IMeshDownloader
    {
        public string meshUrl;
        public int numberOfCalls = 0;
        //public void downloadMesh(string meshUrl, Action<T, float> onProgress, Action<T> onLoad, Action<T> onMaterialsLoad, Action<E> onError = null)
        public void downloadMesh(string meshUrl, Action<ModelLoaderContext, float> onProgress, Action<ModelLoaderContext> onLoad, Action<ModelLoaderContext> onMaterialsLoad)
        {
            numberOfCalls++;
            this.meshUrl = meshUrl;
            onProgress(default, 0.3f);
            onProgress(default, 0.6f);
            onProgress(default, 0.9f);
            onProgress(default, 1f);
            //Debug.Log("on load is");
            //Debug.Log(onLoad);
            //var o = onLoad as Action<AssetLoaderContextStub>;
            //Debug.Log("o is ");
            //Debug.Log(o);
            //o(new AssetLoaderContextStub() { RootGameObject = new GameObject("holi") });
            ////onLoad(new AssetLoaderContextStub() { RootGameObject = new GameObject("holi") });
            onLoad(new ModelLoaderContext()
            {
                loadedObject = new GameObject("holi")
            });
            ////onLoad(new AssetLoaderContextStub() { RootGameObject = new GameObject("holi")} as T);
            onMaterialsLoad(default);
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
