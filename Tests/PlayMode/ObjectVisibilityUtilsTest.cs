using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;

namespace ReupVirtualTwinTests.helpers
{
    public class ObjectVisibilityUtilsTest
    {
        GameObject building;
        IIdGetterController idController = new IdController();

        [SetUp]
        public void SetUp()
        {
            building = StubObjectTreeCreator.CreateMockBuilding();
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
    }
}
