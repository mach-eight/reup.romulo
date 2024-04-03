using NUnit.Framework;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using System;

namespace ReupVirtualTwinTests.controllers
{
    public class InsertObjectControllerTest
    {

        MediatorSpy mediator;
        MeshDownloaderSpy meshDownloader;
        InsertObjectMessagePayload insertObjectMessagePayload;
        InserObjectController controller;

        [SetUp]
        public void SetUp()
        {
            mediator = new MediatorSpy();
            meshDownloader = new MeshDownloaderSpy();
            insertObjectMessagePayload = new InsertObjectMessagePayload()
            {
                objectId = "object-id",
                objectUrl = "object-url",
                selectObjectAfterInsertion = true,
                deselectPreviousSelection = true,
            };
            controller = new InserObjectController(mediator, meshDownloader);
        }

        [Test]
        public void ShouldCreateInsertObjectController()
        {
            Assert.IsNotNull(controller);
        }

        [Test]
        public void ShouldRequestMeshDownload()
        {
            controller.InsertObjectsController(insertObjectMessagePayload);
            Assert.AreEqual(1, meshDownloader.numberOfCalls);
            Assert.AreEqual(insertObjectMessagePayload.objectUrl, meshDownloader.meshUrl);
        }


    }

    class MediatorSpy : IMediator
    {
        public void Notify(ReupEvent eventName)
        {
            throw new System.NotImplementedException();
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            throw new System.NotImplementedException();
        }
    }

    class MeshDownloaderSpy : IMeshDownloader
    {
        public string meshUrl;
        public int numberOfCalls = 0;
        public void downloadMesh<T, E>(string meshUrl, Action<T, float> onProgress, Action<T> onLoad, Action<T> onMaterialsLoad, Action<E> onError = null)
        {
            numberOfCalls++;
            this.meshUrl = meshUrl;
        }
    }
}
