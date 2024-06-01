using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

using ReupVirtualTwin.helperInterfaces;
using Tests.PlayMode.Mocks;
using ReupVirtualTwin.behaviours;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.behaviours
{
    public class SensedObjectHighlighterTest : MonoBehaviour
    {
        GameObject sensedObjectHighlighterGameObject;
        GameObject testObject;
        SensedObjectHighlighter sensedObjectHighter;
        ObjectSensorSpy objectSensor;
        ObjectHighlighterSpy objectHighlighter;

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
            sensedObjectHighter = sensedObjectHighlighterGameObject.AddComponent<SensedObjectHighlighter>();

            objectSensor = sensedObjectHighlighterGameObject.AddComponent<ObjectSensorSpy>();
            sensedObjectHighter.objectSensor = objectSensor;

            objectHighlighter = new ObjectHighlighterSpy();
            sensedObjectHighter.objectHighlighter = objectHighlighter;
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
            Assert.IsNull(objectHighlighter.GetHighlightedObject());
            objectSensor.sensedObject = testObject;
            yield return null;
            Assert.AreEqual(testObject, objectHighlighter.GetHighlightedObject());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRequestHighlightSameObject_only_once()
        {
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

    }
}