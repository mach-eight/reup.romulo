using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.models;
using ReupVirtualTwinTests.mocks;
using ReupVirtualTwinTests.instantiators;

namespace ReupVirtualTwinTests.controllers
{
    public class OriginalSceneControllerTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        IObjectRegistry objectRegistry;
        OriginalSceneController originalSceneController;

        GameObject object1;
        Material material1;
        GameObject object2;
        Material material2;
        GameObject objectWithNoMaterial;
        TexturesManagerSpy texturesManagerSpy;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            objectRegistry = sceneObjects.objectRegistry;
            texturesManagerSpy = new TexturesManagerSpy();
            originalSceneController = new OriginalSceneController(objectRegistry, texturesManagerSpy);
            SetObjectsAndMaterials();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            objectRegistry.ClearRegistry();
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            GameObject.DestroyImmediate(object1);
            GameObject.DestroyImmediate(object2);
            GameObject.DestroyImmediate(objectWithNoMaterial);
            yield return null;
        }

        public void SetObjectsAndMaterials()
        {
            object1 = new GameObject("object1");
            material1 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material1.name = "material1";
            object1.AddComponent<MeshRenderer>().material = material1;
            object1.AddComponent<ObjectInfo>();
            object1.AddComponent<RegisteredIdentifier>();

            object2 = new GameObject("object1");
            material2 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material2.name = "material2";
            object2.AddComponent<MeshRenderer>().material = material2;
            object2.AddComponent<ObjectInfo>();
            object2.AddComponent<RegisteredIdentifier>();

            objectWithNoMaterial = new GameObject("objectWithNoMaterial");
            objectWithNoMaterial.AddComponent<ObjectInfo>();
        }

        [UnityTest]
        public IEnumerator ShouldApplyOriginalMaterialsAsProtectedMaterialsInTextureManager()
        {
            Assert.AreEqual(material1.name, object1.GetComponent<MeshRenderer>().sharedMaterial.name);
            Assert.AreEqual(material2.name, object2.GetComponent<MeshRenderer>().sharedMaterial.name);
            Assert.IsNull(objectWithNoMaterial.GetComponent<MeshRenderer>());
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.name = "newMaterial";
            object1.GetComponent<MeshRenderer>().material = newMaterial;
            object1.GetComponent<IObjectInfo>().materialWasChanged = true;
            object2.GetComponent<MeshRenderer>().material = newMaterial;
            object2.GetComponent<IObjectInfo>().materialWasChanged = true;
            yield return null;
            Assert.AreNotEqual(material1.name, object1.GetComponent<MeshRenderer>().sharedMaterial.name);
            Assert.AreNotEqual(material2.name, object2.GetComponent<MeshRenderer>().sharedMaterial.name);
            Assert.IsNull(objectWithNoMaterial.GetComponent<MeshRenderer>());
            originalSceneController.RestoreOriginalScene();
            yield return null;
            Assert.AreEqual(2, texturesManagerSpy.calledToApplyProtectedMaterials.Count);
            Assert.AreEqual(material1, texturesManagerSpy.calledToApplyProtectedMaterials[0]);
            Assert.AreEqual(material2, texturesManagerSpy.calledToApplyProtectedMaterials[1]);
            Assert.AreEqual(object1, texturesManagerSpy.calledObjectsToApplyProtectedMaterial[0]);
            Assert.AreEqual(object2, texturesManagerSpy.calledObjectsToApplyProtectedMaterial[1]);
            Assert.IsTrue(object1.GetComponent<IObjectInfo>().materialWasRestored);
            Assert.IsTrue(object2.GetComponent<IObjectInfo>().materialWasRestored);
            Assert.IsNull(objectWithNoMaterial.GetComponent<MeshRenderer>());
            Assert.IsFalse(objectWithNoMaterial.GetComponent<IObjectInfo>().materialWasRestored);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotChangeMaterialOfObjectThatAlreadyHasOriginalMaterial()
        {
            Assert.IsFalse(object1.GetComponent<IObjectInfo>().materialWasRestored);
            Assert.IsFalse(object2.GetComponent<IObjectInfo>().materialWasRestored);
            originalSceneController.RestoreOriginalScene();
            yield return null;
            Assert.IsFalse(object1.GetComponent<IObjectInfo>().materialWasRestored);
            Assert.IsFalse(object2.GetComponent<IObjectInfo>().materialWasRestored);
            yield return null;
        }

    }
}
