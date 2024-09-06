using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using System.Linq;

namespace ReupVirtualTwinTests.helpers
{
    public class ObjectVisibilityUtilsTest
    {
        GameObject building;
        IIdGetterController idController = new IdController();
        static SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;

        [SetUp]
        public void SetUp()
        {
            building = StubObjectTreeCreator.CreateMockBuilding(5);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(building);
        }

        public void AssertAllObjectsVisible(GameObject parent)
        {
            Dictionary<string, bool> visibilityStates = ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(parent, idController);
            foreach(var kvp in visibilityStates)
            {
                Assert.IsTrue(kvp.Value);
            }
        }

        public List<string> MakeInvisibleSomeObjects()
        {
            List<GameObject> someInvisibleObjects = new List<GameObject>()
            {
                building,
                building.transform.GetChild(0).gameObject,
                building.transform.GetChild(2).gameObject,
            };
            List<string> invisibleItemsIds = someInvisibleObjects.Select(obj => idController.GetIdFromObject(obj)).ToList();
            foreach(GameObject obj in someInvisibleObjects)
            {
                visibilityManager.Hide(obj, false);
            }
            return invisibleItemsIds;
        }

        private static void AssertVisibilityStates(List<string> invisibleItemsIds, Dictionary<string, bool> visibilityStates)
        {
            foreach (var kvp in visibilityStates)
            {
                if (invisibleItemsIds.IndexOf(kvp.Key) >= 0)
                {
                    Assert.IsFalse(kvp.Value);
                    continue;
                }
                Assert.IsTrue(kvp.Value);
            }
        }

        [Test]
        public void ShouldReturnObjectsVisibilityState_with_all_objects_visible()
        {
            AssertAllObjectsVisible(building);
        }

        [Test]
        public void ShouldReturnObjectsVisibilityState_with_some_objects_visible()
        {
            List<string> invisibleItemsIds = MakeInvisibleSomeObjects();
            Dictionary<string, bool> visibilityStates = ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, idController);
            AssertVisibilityStates(invisibleItemsIds, visibilityStates);
        }

        [Test]
        public void ShouldApplyVisibility()
        {
            List<string> invisibleItemsIds = MakeInvisibleSomeObjects();
            Dictionary<string, bool> visibilityStatesBefore = ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, idController);
            visibilityManager.Show(building, true);
            AssertAllObjectsVisible(building);
            ObjectVisibilityUtils.ApplyVisibilityState(building, visibilityStatesBefore, idController);
            Dictionary<string, bool> visibilityStatesAfter = ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, idController);
            Assert.AreEqual(visibilityStatesBefore, visibilityStatesAfter);
        }
    }
}
