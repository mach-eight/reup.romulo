using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.managers;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwinTests
{
    public class HighlightSelectableObjectTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        SelectSelectableObject selectSelectableObjectBehavior;
        SelectableObjectHighlighterMock selectableObjectHighligherMock;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            selectSelectableObjectBehavior = sceneObjects.selectSelectableObject;
            yield return null; // this yield is necessary so all game objects run the Start() method
            selectableObjectHighligherMock = new SelectableObjectHighlighterMock();
            selectSelectableObjectBehavior.selectableObjectsHighlighter = selectableObjectHighligherMock;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        class SelectableObjectHighlighterMock : ISelectableObjectsHighlighter
        {
            public bool highlighRequested = false;
            public void HighlightSelectableObjects()
            {
                highlighRequested = true;
            }
        }

        [UnityTest]
        public IEnumerator ShouldNotHighlightSelectableObjects_When_Tap_AndNot_InEditMode()
        {
            ReupSceneInstantiator.SetEditMode(sceneObjects, false);
            selectSelectableObjectBehavior.MissObject();
            yield return null;
            Assert.IsFalse(selectableObjectHighligherMock.highlighRequested);
        }

        [UnityTest]
        public IEnumerator ShouldHighlightSelectableObjects_When_Tap_And_InEditMode()
        {
            ReupSceneInstantiator.SetEditMode(sceneObjects, true);
            selectSelectableObjectBehavior.MissObject();
            yield return null;
            Assert.IsTrue(selectableObjectHighligherMock.highlighRequested);
        }
    }
}
