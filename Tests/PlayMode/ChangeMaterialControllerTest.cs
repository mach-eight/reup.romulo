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
using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwinTests.controllers
{
    public class ChangeMaterialControllerTest
    {
        TextureDownloaderSpy textureDownloaderSpy;
        ChangeMaterialController controller;
        JObject messagePayload;
        SomeObjectWithMaterialRegistrySpy objectRegistry;
        MediatorSpy mediatorSpy;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            mediatorSpy = new MediatorSpy();
            textureDownloaderSpy = new TextureDownloaderSpy();
            objectRegistry = new SomeObjectWithMaterialRegistrySpy();
            controller = new ChangeMaterialController(textureDownloaderSpy, objectRegistry, mediatorSpy);
            messagePayload = new JObject()
            {
                { "material_id", 1234567890 },
                { "material_url", "material-url.com" },
                { "object_ids", new JArray(new string[] { "id-0", "id-1" }) }
            };
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            objectRegistry.DestroyAllObjects();
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
            public JObject changeMaterialInfo = null;
            public void Notify(ReupEvent eventName)
            {
                throw new NotImplementedException();
            }

            public void Notify<T>(ReupEvent eventName, T payload)
            {
                if (eventName == ReupEvent.objectMaterialChanged)
                {
                    changeMaterialInfo = payload as JObject;
                }
            }
        }

        private List<Material> GetMaterialsFromObjects(List<GameObject> objects)
        {
            List<Material> materials = new();
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    materials.Add(objects[i].GetComponent<Renderer>().material);
                }
            }
            return materials;
        }
        void AssignFakeColorMetaDataToObjects(List<GameObject> objects, string colorCode)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<MeshRenderer>() != null)
                {
                    ObjectMetaDataUtils.AssignColorMetaDataToObject(objects[i], colorCode);
                }
            }
        }

        [UnityTest]
        public IEnumerator ShouldCreateTheController()
        {
            Assert.IsNotNull(controller);
            yield return null;
        }

        [Test]
        public async Task ShouldRequestDownloadMaterialTexture()
        {
            await controller.ChangeObjectMaterial(messagePayload);
            Assert.AreEqual(messagePayload["material_url"].ToString(), textureDownloaderSpy.url);
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

        [Test]
        public async Task ShouldNotNotifyMediator_When_MaterialsChange()
        {
            await controller.ChangeObjectMaterial(messagePayload);
            Assert.IsNull(mediatorSpy.changeMaterialInfo);
        }

        [Test]
        public async Task ShouldSaveMaterialId_In_ObjectsMetaData()
        {
            List<JToken> objectsMaterialId = ObjectMetaDataUtils.GetMetaDataValuesFromObjects(
                objectRegistry.objects, "appearance.material_id");
            AssertUtils.AssertAllAreNull(objectsMaterialId);
            await controller.ChangeObjectMaterial(messagePayload);
            AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<int>(
                objectRegistry.objects,
                "appearance.material_id",
                messagePayload["material_id"].ToObject<int>());
        }

        [Test]
        public async Task ShouldDeleteColorMetaData_when_applyingMaterialIdMetaData()
        {
            string fakeColorCode = "fake-color-code";
            AssignFakeColorMetaDataToObjects(objectRegistry.objects, fakeColorCode);
            AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<string>(
                objectRegistry.objects,
                "appearance.color",
                fakeColorCode);
            await controller.ChangeObjectMaterial(messagePayload);
            List<JToken> objectsColorCode = ObjectMetaDataUtils.GetMetaDataValuesFromObjects(
                objectRegistry.objects, "appearance.color");
            AssertUtils.AssertAllAreNull(objectsColorCode);
            AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<int>(
                objectRegistry.objects,
                "appearance.material_id",
                messagePayload["material_id"].ToObject<int>());
        }

        [Test]
        public async Task ShouldSetChangedMaterialInObjectInfo()
        {
            await controller.ChangeObjectMaterial(messagePayload);
            AssertUtils.AssertAllObjectsWithMeshRendererHaveSetChangedMaterial(objectRegistry.objects);
        }

    }
}
