using System.Collections.Generic;
using NUnit.Framework;
using ReupVirtualTwin.models;
using UnityEngine;

namespace ReupVirtualTwinTests.editor
{
    public class RegisteredIdentifierEditorTest : MonoBehaviour
    {
        List<GameObject> gameObjects;
        GameObject objectRegistryGameObject;
        ObjectRegistry objectRegistry;
        [SetUp]
        public void SetUp()
        {
            gameObjects = new List<GameObject>();
            objectRegistryGameObject = new GameObject("objectRegistry");
            objectRegistry = objectRegistryGameObject.AddComponent<ObjectRegistry>();
        }
        [TearDown]
        public void TearDown()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                GameObject.DestroyImmediate(gameObject);
            }
            GameObject.DestroyImmediate(objectRegistryGameObject);
        }
        [Test]
        public void ShouldRaiseException_if_attemptToRegister2ObjectsWithSameManualId()
        {
            string repeatedId = "repeated-id";
            for (int i = 0; i < 2; i++)
            {
                GameObject testObj = new GameObject($"testObj{i}");
                RegisteredIdentifier registeredIdentifier = testObj.AddComponent<RegisteredIdentifier>();
                registeredIdentifier.objectRegistry = objectRegistry;
                registeredIdentifier.manualId = repeatedId;
                gameObjects.Add(testObj);
            }
            gameObjects[0].GetComponent<RegisteredIdentifier>().Awake();
            Assert.That(() => gameObjects[1].GetComponent<RegisteredIdentifier>().Awake(), Throws.Exception);
        }
    }

}