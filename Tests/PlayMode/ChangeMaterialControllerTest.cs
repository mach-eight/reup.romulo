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

namespace ReupVirtualTwinTests.controllers
{
    public class ChangeMaterialControllerTest
    {

        TextureDownloaderSpy textureDownloaderSpy;
        ChangeMaterialController controller;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            textureDownloaderSpy = new();
            controller = new ChangeMaterialController(textureDownloaderSpy);
            yield return null;
        }

        private class TextureDownloaderSpy : ITextureDownloader
        {
            public string url;
            public Task<Texture2D> DownloadTextureFromUrl(string url)
            {
                this.url = url;
                return Task.FromResult(new Texture2D(1, 1));
            }
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
            ChangeMaterialMessagePayload messagePayload = new()
            {
                material_url = "material-url.com",
                object_ids = new string[] { "id-0", "id-1" },
            };
            controller.ChangeObjectMaterial(messagePayload);
            Assert.AreEqual(messagePayload.material_url, textureDownloaderSpy.url);
            yield return null;
        }
    }
}
