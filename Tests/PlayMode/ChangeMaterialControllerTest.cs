using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Threading.Tasks;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using Tests.PlayMode.Mocks;
using ReupVirtualTwin.modelInterfaces;
using System.Linq;

namespace ReupVirtualTwinTests.controllers
{
    public class ChangeMaterialControllerTest
    {

        TextureDownloaderSpy textureDownloaderSpy;
        ChangeMaterialController controller;
        ChangeMaterialMessagePayload messagePayload;
        SomeObjectWithMaterialRegistrySpy objectRegistry = new();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            textureDownloaderSpy = new TextureDownloaderSpy();
            controller = new ChangeMaterialController(textureDownloaderSpy, objectRegistry);
            messagePayload = new()
            {
                material_url = "material-url.com",
                object_ids = new string[] { "id-0", "id-1" },
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
            Assert.AreEqual(messagePayload.material_url, textureDownloaderSpy.url);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldChangeMaterialsOfObjects()
        {
            List<Material> originalMaterials = GetMaterialsFromObjects(objectRegistry.objects);
            controller.ChangeObjectMaterial(messagePayload);
            List<Material> newMaterials = GetMaterialsFromObjects(objectRegistry.objects);
            Assert.AreEqual(originalMaterials.Count, newMaterials.Count);
            for (int i = 0; i < originalMaterials.Count; i++)
            {
                Assert.AreNotEqual(originalMaterials[i], newMaterials[i]);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldAssignMaterialsWithDownloadedTexture()
        {
            controller.ChangeObjectMaterial(messagePayload);
            List<Material> newMaterials = GetMaterialsFromObjects(objectRegistry.objects);
            for(int i = 0; i < newMaterials.Count; i++)
            {
                Assert.AreEqual(textureDownloaderSpy.texture, newMaterials[i].GetTexture("_BaseMap"));
            }
            yield return null;
        }

    }
}
