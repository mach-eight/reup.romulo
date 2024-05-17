using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.webRequestersInterfaces;
using Tests.PlayMode.Mocks;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwinTests.controllers
{
    public class ChangeMaterialControllerTest
    {
        TextureDownloaderSpy textureDownloaderSpy;
        ChangeMaterialController controller;
        Dictionary<string, object> messagePayload;
        SomeObjectWithMaterialRegistrySpy objectRegistry;
        MediatorSpy mediatorSpy;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            mediatorSpy = new MediatorSpy();
            textureDownloaderSpy = new TextureDownloaderSpy();
            objectRegistry = new SomeObjectWithMaterialRegistrySpy();
            controller = new ChangeMaterialController(textureDownloaderSpy, objectRegistry, mediatorSpy);
            messagePayload = new Dictionary<string, object>()
            {
                { "material_url", "material-url.com" },
                { "object_ids", new string[] { "id-0", "id-1" } }
            };
            yield return null;
        }

        private class TextureDownloaderSpy : ITextureDownloader
        {
            public string url;
            public Texture2D texture = new Texture2D(1,1);
            public async Task<Texture2D> DownloadTextureFromUrl(string url)
            {
                this.url = url;
                await Task.Delay(1);
                return texture;
            }
        }

        private class MediatorSpy : IMediator
        {
            public Dictionary<string, object> changeMaterialInfo;
            public void Notify(ReupEvent eventName)
            {
                throw new NotImplementedException();
            }

            public void Notify<T>(ReupEvent eventName, T payload)
            {
                Debug.Log("the notification");
                Debug.Log(eventName);
                Debug.Log(payload.GetType());
                if (eventName == ReupEvent.objectMaterialChanged)
                {
                    Debug.Log("assigning thing");
                    changeMaterialInfo = payload as Dictionary<string, object>;
                }
            }
        }

        private List<Material> GetMaterialsFromObjects(List<GameObject> objects)
        {
            List<Material> originalMaterials = new();
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    originalMaterials.Add(objects[i].GetComponent<Renderer>().material);
                }
            }
            return originalMaterials;
        }

        [UnityTest]
        public IEnumerator ShouldCreateTheController()
        {
            Assert.IsNotNull(controller);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRequestDownloadMaterialTexture()
        {
            controller.ChangeObjectMaterial(messagePayload);
            Assert.AreEqual(messagePayload["material_url"], textureDownloaderSpy.url);
            yield return null;
        }

        [Test]
        public async Task ShouldChangeMaterialsOfObjects()
        {
            List<Material> originalMaterials = GetMaterialsFromObjects(objectRegistry.objects);
            await controller.ChangeObjectMaterial(messagePayload);
            List<Material> newMaterials = GetMaterialsFromObjects(objectRegistry.objects);
            Assert.AreEqual(originalMaterials.Count, newMaterials.Count);
            for (int i = 0; i < originalMaterials.Count; i++)
            {
                Assert.AreNotEqual(originalMaterials[i], newMaterials[i]);
            }
        }

        [Test]
        public async Task ShouldAssignMaterialsWithDownloadedTexture()
        {
            await controller.ChangeObjectMaterial(messagePayload);
            List<Material> newMaterials = GetMaterialsFromObjects(objectRegistry.objects);
            for(int i = 0; i < newMaterials.Count; i++)
            {
                Assert.AreEqual(textureDownloaderSpy.texture, newMaterials[i].GetTexture("_BaseMap"));
            }
        }

        [Test]
        public async Task ShouldNotifyMediator_When_MaterialsChange()
        {
            await controller.ChangeObjectMaterial(messagePayload);
            Assert.AreEqual(messagePayload["material_url"], mediatorSpy.changeMaterialInfo["material_url"]);
            Assert.AreEqual(messagePayload["object_ids"], mediatorSpy.changeMaterialInfo["object_ids"]);
        }

    }
}
