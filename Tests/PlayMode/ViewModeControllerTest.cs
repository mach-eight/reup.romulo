using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TestTools;

using ReupVirtualTwin.controllers;
using NUnit.Framework;
using ReupVirtualTwin.enums;
using System.Linq;

namespace ReupVirtualTwinTests.playmode.controller
{
    public class ViewModeControllerTest: MonoBehaviour
    {
        ViewModeController viewModelController;
        GameObject fpvCamera;
        GameObject dhvCamera;
        List<GameObject> cameras;

        [SetUp]
        public void SetUp()
        {
            CreateCameras();
            viewModelController = new ViewModeController(fpvCamera, dhvCamera);
        }

        [TearDown]
        public void TearDown()
        {
            Destroy(fpvCamera);
            Destroy(dhvCamera);
        }

        private void CreateCameras()
        {
            fpvCamera = new GameObject();
            dhvCamera = new GameObject();
            cameras = new List<GameObject> { fpvCamera, dhvCamera };
        }

        private void AssertOnlyOneCameraIsActive(GameObject expectedActiveCamera)
        {
            Assert.IsTrue(expectedActiveCamera.activeSelf);
            List<GameObject> restOfCameras = cameras.Where(camera => camera != expectedActiveCamera).ToList();
            foreach (GameObject camera in restOfCameras)
            {
                Assert.IsFalse(camera.activeSelf);
            }
        }

        [Test]
        public void ShouldHaveFPVActivatedByDefault()
        {
            Assert.AreEqual(viewModelController.viewMode, ViewMode.FPV);
        }

        [Test]
        public void ShouldChangeViewModeToHDV()
        {
            viewModelController.ActivateDHV();
            Assert.AreEqual(viewModelController.viewMode, ViewMode.DHV);
        }

        [Test]
        public void ShouldChangeBackViewModeToFPV()
        {
            viewModelController.viewMode = ViewMode.DHV;
            Assert.AreEqual(viewModelController.viewMode, ViewMode.DHV);
            viewModelController.ActivateFPV();
            Assert.AreEqual(viewModelController.viewMode, ViewMode.FPV);
        }

        [Test]
        public void ShouldActivateFPVCamera_when_changeViewModeToFPV()
        {
            fpvCamera.SetActive(false);
            Assert.IsFalse(fpvCamera.activeSelf);
            viewModelController.ActivateFPV();
            AssertOnlyOneCameraIsActive(fpvCamera);
        }

        [Test]
        public void ShouldActivateDHVCamera_when_changeViewModeToDHV()
        {
            dhvCamera.SetActive(false);
            Assert.IsFalse(dhvCamera.activeSelf);
            viewModelController.ActivateDHV();
            AssertOnlyOneCameraIsActive(dhvCamera);
        }

    }
}
