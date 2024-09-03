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
using ReupVirtualTwin.helperInterfaces;
using System.Linq;

namespace ReupVirtualTwinTests.controllers
{
    public class ChangeMaterialControllerTest
    {
        TextureDownloaderSpy textureDownloaderSpy;
        ChangeMaterialController controller;
        JObject messagePayload;
        SomeObjectWithMaterialRegistrySpy objectRegistry;
        MaterialScalerSpy materialScalerSpy;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            textureDownloaderSpy = new TextureDownloaderSpy();
            objectRegistry = new SomeObjectWithMaterialRegistrySpy();
            controller = new ChangeMaterialController(textureDownloaderSpy, objectRegistry);
            materialScalerSpy = new MaterialScalerSpy();
            controller.materialScaler = materialScalerSpy;
            messagePayload = new JObject()
            {
                { "objectIds", new JArray(new string[] { "id-0", "id-1" }) },
                { "material", new JObject()
                    {
                        { "id", 1234567890 },
                        { "texture", "material-url.com" },
                        { "widthMilimeters", 2000 },
                        { "heightMilimeters", 1500 },
                    }
                }
            };
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            objectRegistry.DestroyAllObjects();
            yield return null;
        }

        private class MaterialScalerSpy : IMaterialScaler
        {
            public int callCount = 0;
            public List<GameObject> calledObjects = new List<GameObject>();
            public List<Vector2> calledDimensions = new List<Vector2>();
            public void AdjustUVScaleToDimensions(GameObject obj, Vector2 dimensionsInM)
            {
                callCount++;
                calledObjects.Add(obj);
                calledDimensions.Add(dimensionsInM);
            }
        }

        private class TextureDownloaderSpy : ITextureDownloader
        {
            public string url;
            public bool shouldFail= false;
            public Texture2D texture = new Texture2D(1,1);
            public async Task<Texture2D> DownloadTextureFromUrl(string url)
            {
                this.url = url;
                await Task.Delay(1);
                if (shouldFail)
                {
                    return null;
                }
                return texture;
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
            Assert.AreEqual(messagePayload["material"]["texture"].ToString(), textureDownloaderSpy.url);
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
        public async Task ShouldReturnSuccess_When_MaterialsChange()
        {
            var result = await controller.ChangeObjectMaterial(messagePayload);

            Assert.IsTrue(result.IsSuccess, "The material change operation should succeed.");

        }

        [Test]
        public async Task ShouldReturnFailure_When_TextureDownloadFails()
        {
            textureDownloaderSpy.shouldFail = true;
            var result = await controller.ChangeObjectMaterial(messagePayload);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Error downloading image from {messagePayload["material"]["texture"]}", result.Error);

        }

        [Test]
        public async Task ShouldSaveMaterialId_In_ObjectsMetaData()
        {
            List<JToken> objectsMaterialId = ObjectMetaDataUtils.GetMetaDataValuesFromObjects(
                objectRegistry.objects, "appearance.materialId");
            AssertUtils.AssertAllAreNull(objectsMaterialId);
            await controller.ChangeObjectMaterial(messagePayload);
            AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<int>(
                objectRegistry.objects,
                "appearance.materialId",
                messagePayload["material"]["id"].ToObject<int>());
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
                "appearance.materialId",
                messagePayload["material"]["id"].ToObject<int>());
        }

        [Test]
        public async Task ShouldSetChangedMaterialInObjectInfo()
        {
            await controller.ChangeObjectMaterial(messagePayload);
            AssertUtils.AssertAllObjectsWithMeshRendererHaveSetChangedMaterial(objectRegistry.objects);
        }

        [Test]
        public async Task ShouldRequestMaterialScalerToScaleObjectsMaterial()
        {
            List<GameObject> objectsWithMeshRenderer = ObjectUtils.FilterForObjectsWithMeshRenderer(objectRegistry.objects);
            int numberOfObjectsWithMesh = objectsWithMeshRenderer.Count;
            List<Vector2> expectedDimensionCalls = Enumerable.Repeat<Vector2>(new Vector2(2000, 1500), numberOfObjectsWithMesh).ToList();
            await controller.ChangeObjectMaterial(messagePayload);
            Assert.AreEqual(numberOfObjectsWithMesh, materialScalerSpy.callCount);
            Assert.AreEqual(objectsWithMeshRenderer, materialScalerSpy.calledObjects);
            Assert.AreEqual(expectedDimensionCalls, materialScalerSpy.calledDimensions);

        }

    }
}

