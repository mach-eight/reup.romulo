using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.managers;
using NUnit.Framework;
using ReupVirtualTwin.enums;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.playmode.managers
{
    public class ViewModeManagerTest: MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        ViewModeManager viewModelManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            viewModelManager = sceneObjects.viewModeManager;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [Test]
        public void ShouldHaveFPVActivatedByDefault()
        {
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.FPV);
        }

        //[Test]
    }
}
