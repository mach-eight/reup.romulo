using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.behaviours;
using UnityEngine.TestTools;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwinTests.mocks;

namespace ReupVirtualTwinTests.behaviours
{
    public class SensedObjectHighlighterTest : MonoBehaviour
    {
        GameObject sensedObjectHighlighterGameObject;
        GameObject testObject;
        SensedObjectHighlighter sensedObjectHighlighter;
        ObjectSensorSpy objectSensor;
        ObjectHighlighterSpy objectHighlighter;
        SelectedObjectsManagerSpy isObjectPartOfSelection;

        class SelectedObjectsManagerSpy : IIsObjectPartOfSelection
        {
            public bool isObjectPartOfSelection = false;
            public bool IsObjectPartOfSelection(GameObject obj)
            {
                return isObjectPartOfSelection;
            }
        }

        class ObjectSensorSpy : MonoBehaviour, IObjectSensor
        {
            public GameObject sensedObject;
            public GameObject Sense()
            {
                return sensedObject;
            }
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            testObject = new GameObject("testObject");
            sensedObjectHighlighterGameObject = new GameObject("sensedObjectHighlighterGameObject");
            sensedObjectHighlighter = sensedObjectHighlighterGameObject.AddComponent<SensedObjectHighlighter>();

            objectSensor = sensedObjectHighlighterGameObject.AddComponent<ObjectSensorSpy>();
            sensedObjectHighlighter.objectSensor = objectSensor;

            objectHighlighter = new ObjectHighlighterSpy();
            sensedObjectHighlighter.objectHighlighter = objectHighlighter;

            isObjectPartOfSelection = new SelectedObjectsManagerSpy();
            sensedObjectHighlighter.selectedObjectsManager = isObjectPartOfSelection;
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Destroy(testObject);
            Destroy(sensedObjectHighlighterGameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotHighlightObject_if_noObjectIsSensed()
        {
            yield return null;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldHighlightObject_if_ObjectIsSensed()
        {
            sensedObjectHighlighter.enableHighlighting = true;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.AreEqual(testObject, objectHighlighter.GetHighlightedObject());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRequestHighlightSameObject_only_once()
        {
            sensedObjectHighlighter.enableHighlighting = true;
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.AreEqual(testObject, objectHighlighter.GetHighlightedObject());
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            Assert.AreEqual(1, objectHighlighter.GetHighlightCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldStopHighlightingObject_when_objectIsNotSensed()
        {
            sensedObjectHighlighter.enableHighlighting = true;
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.AreEqual(testObject, objectHighlighter.GetHighlightedObject());
            yield return null;
            objectSensor.sensedObject = null;
            yield return null;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(1, objectHighlighter.GetHighlightCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotHighlightObject_when_switchIsOff()
        {
            sensedObjectHighlighter.enableHighlighting = false;
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(0, objectHighlighter.GetHighlightCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldStopHighlightingObject_when_switchIsTurnedOff()
        {
            sensedObjectHighlighter.enableHighlighting = true;
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.AreEqual(testObject, objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(1, objectHighlighter.GetHighlightCount());

            sensedObjectHighlighter.enableHighlighting = false;
            yield return null;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(1, objectHighlighter.GetHighlightCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldStartHighlightingObject_when_switchIsTurnedOn()
        {
            sensedObjectHighlighter.enableHighlighting = false;
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(0, objectHighlighter.GetHighlightCount());

            sensedObjectHighlighter.enableHighlighting = true;
            yield return null;
            Assert.AreEqual(testObject, objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(1, objectHighlighter.GetHighlightCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotHighlightObject_if_objectIsPartOfSelection()
        {
            sensedObjectHighlighter.enableHighlighting = true;
            isObjectPartOfSelection.isObjectPartOfSelection = true;
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            Assert.AreEqual(0, objectHighlighter.GetHighlightCount());
            yield return null;
        }

    }
}
