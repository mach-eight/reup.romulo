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

        [Test]
        public void ShouldReturnObjectsVisibilityState_with_all_objects_visible()
        {
            Dictionary<string, bool> objectStates = ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, idController);
            foreach(var kvp in objectStates)
            {
                Assert.IsTrue(kvp.Value);
            }
        }

        [Test]
        public void ShouldReturnObjectsVisibilityState_with_some_objects_visible()
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
            Dictionary<string, bool> objectStates = ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, idController);
            foreach(var kvp in objectStates)
            {
                if (invisibleItemsIds.IndexOf(kvp.Key) >= 0)
                {
                    Assert.IsFalse(kvp.Value);
                    continue;
                }
                Assert.IsTrue(kvp.Value);
            }
        }
    }
}
