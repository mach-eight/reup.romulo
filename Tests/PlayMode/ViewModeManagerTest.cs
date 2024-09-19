using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TestTools;

using NUnit.Framework;
using ReupVirtualTwin.enums;
using System.Linq;
using ReupVirtualTwin.managers;
using ReupVirtualTwinTests.utils;
using System.Collections;

namespace ReupVirtualTwinTests.playmode.managers
{
    public class ViewModeManagerTest: MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        ViewModeManager viewModelManager;
        GameObject fpvCamera;
        GameObject dhvCamera;
        List<GameObject> cameras;
        GameObject character;

        [UnitySetUp]
        public IEnumerator SetUp()
        {

            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            viewModelManager = sceneObjects.viewModeManager;
            character = sceneObjects.character;
            dhvCamera = sceneObjects.dhvCamera;
            fpvCamera = sceneObjects.fpvCamera;
            cameras = new List<GameObject> { fpvCamera, dhvCamera };
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        private void AssertOnlyOneCameraIsActive(GameObject expectedActiveCamera)
        {
            Assert.IsTrue(expectedActiveCamera.activeInHierarchy);
            List<GameObject> restOfCameras = cameras.Where(camera => camera != expectedActiveCamera).ToList();
            foreach (GameObject camera in restOfCameras)
            {
                Assert.IsFalse(camera.activeInHierarchy);
            }
        }

        [Test]
        public void ShouldHaveFPVActivatedByDefault()
        {
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.FPV);
        }

        [Test]
        public void ShouldChangeViewModeToHDV()
        {
            viewModelManager.ActivateDHV();
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.DHV);
        }

        [Test]
        public void ShouldChangeBackViewModeToFPV()
        {
            viewModelManager.viewMode = ViewMode.DHV;
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.DHV);
            viewModelManager.ActivateFPV();
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.FPV);
        }

        [Test]
        public void ShouldActivateFPVCamera_when_changeViewModeToFPV()
        {
            character.SetActive(false);
            Assert.IsFalse(fpvCamera.activeInHierarchy);
            viewModelManager.ActivateFPV();
            AssertOnlyOneCameraIsActive(fpvCamera);
        }

        [Test]
        public void ShouldActivateDHVCamera_when_changeViewModeToDHV()
        {
            dhvCamera.SetActive(false);
            Assert.IsFalse(dhvCamera.activeSelf);
            viewModelManager.ActivateDHV();
            AssertOnlyOneCameraIsActive(dhvCamera);
        }

        [Test]
        public void CharacterShouldBeDeactivated_when_inDHV()
        {
            Assert.IsTrue(character.activeSelf);
            viewModelManager.ActivateDHV();
            Assert.IsFalse(character.activeSelf);
            viewModelManager.ActivateFPV();
            Assert.IsTrue(character.activeSelf);
        }

    }
}
