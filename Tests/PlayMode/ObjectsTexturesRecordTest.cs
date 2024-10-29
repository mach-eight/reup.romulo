using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using ReupVirtualTwinTests.utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.models
{
    public class ObjectsTexturesRecordTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        IObjectRegistry objectRegistry;
        GameObject obj1;
        GameObject obj2;
        IdController idController;
        ObjectsTexturesRecord objectsTexturesRecord;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            objectRegistry = sceneObjects.objectRegistry;
            objectsTexturesRecord = new ObjectsTexturesRecord();
            idController = new IdController();
            obj1 = new GameObject().AddComponent<MeshRenderer>().gameObject;
            obj2 = new GameObject().AddComponent<MeshRenderer>().gameObject;
            idController.AssignIdToObject(obj1);
            idController.AssignIdToObject(obj2);
            yield return null;
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            GameObject.DestroyImmediate(obj1);
            GameObject.DestroyImmediate(obj2);
            objectRegistry.ClearRegistry();
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        private Material CreateDefaultMaterial()
        {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Texture2D defaultTexture = new Texture2D(1, 1);
            material.SetTexture("_BaseMap", defaultTexture);
            return material;
        }

        [UnityTest]
        public IEnumerator ShouldUpdateRecords()
        {
            string objId = obj1.GetInstanceID().ToString();
            Renderer renderer = obj1.GetComponent<Renderer>();
            renderer.material = CreateDefaultMaterial();
            Texture dummyTexture = obj1.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            yield return null;

            objectsTexturesRecord.UpdateRecords(obj1);
            yield return null;

            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[objId]);
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(objId));
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Count);
        }

        [UnityTest]
        public IEnumerator ShouldCleanRecords()
        {
            string objId = obj2.GetInstanceID().ToString();
            Renderer renderer = obj2.GetComponent<Renderer>();
            renderer.material = CreateDefaultMaterial();
            Texture dummyTexture = obj2.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            yield return null;

            objectsTexturesRecord.UpdateRecords(obj2);
            yield return null;
            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[objId]);
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(objId));
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Count);

            objectsTexturesRecord.CleanRecords(obj2);
            yield return null;
            Assert.AreEqual(false, objectsTexturesRecord.objectIdsToTexturesRecord.ContainsKey(objId));
            Assert.AreEqual(false, objectsTexturesRecord.texturesToObjectIdsRecord.ContainsKey(dummyTexture));
            Assert.AreEqual(0, objectsTexturesRecord.texturesToObjectIdsRecord.Count);
        }

        [UnityTest]
        public IEnumerator ShouldPreserveTextureWhenThereAreObjectsAreAssociatedWithTexture()
        {
            string obj1Id = obj1.GetInstanceID().ToString();
            Material dummyMaterial = CreateDefaultMaterial();
            Renderer rendererObj1 = obj1.GetComponent<Renderer>();
            rendererObj1.material = dummyMaterial;
            string obj2Id = obj2.GetInstanceID().ToString();
            Renderer rendererObj2 = obj2.GetComponent<Renderer>();
            rendererObj2.material = dummyMaterial;
            Texture dummyTexture = obj1.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            yield return null;

            objectsTexturesRecord.UpdateRecords(obj1);
            objectsTexturesRecord.UpdateRecords(obj2);
            yield return null;

            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[obj1Id]);
            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[obj2Id]);
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord.Count);
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(obj1Id));
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(obj2Id));
            Assert.AreEqual(2, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Count);
            
            objectsTexturesRecord.CleanRecords(obj1);
            yield return null;

            Assert.AreEqual(false, objectsTexturesRecord.objectIdsToTexturesRecord.ContainsKey(obj1Id));
            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[obj2Id]);
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord.Count);
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(obj2Id));
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Count);
        }

        [UnityTest]
        public IEnumerator ShouldDeleteTextureIfThereAreNoObjectsAssociatedWithTexture()
        {
            string obj1Id = obj1.GetInstanceID().ToString();
            Material dummyMaterial = CreateDefaultMaterial();
            Renderer rendererObj1 = obj1.GetComponent<Renderer>();
            rendererObj1.material = dummyMaterial;
            string obj2Id = obj2.GetInstanceID().ToString();
            Renderer rendererObj2 = obj2.GetComponent<Renderer>();
            rendererObj2.material = dummyMaterial;
            Texture dummyTexture = obj1.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            yield return null;

            objectsTexturesRecord.UpdateRecords(obj1);
            objectsTexturesRecord.UpdateRecords(obj2);
            yield return null;

            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[obj1Id]);
            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[obj2Id]);
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord.Count);
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(obj1Id));
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(obj2Id));
            Assert.AreEqual(2, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Count);
            
            objectsTexturesRecord.CleanRecords(obj1);
            yield return null;
            Assert.AreEqual(false, objectsTexturesRecord.objectIdsToTexturesRecord.ContainsKey(obj1Id));
            Assert.AreEqual(dummyTexture, objectsTexturesRecord.objectIdsToTexturesRecord[obj2Id]);
            Assert.AreEqual(1, objectsTexturesRecord.texturesToObjectIdsRecord.Count);
            Assert.AreEqual(true, objectsTexturesRecord.texturesToObjectIdsRecord[dummyTexture].Contains(obj2Id));

            objectsTexturesRecord.CleanRecords(obj2);
            yield return null;

            Assert.AreEqual(false, objectsTexturesRecord.objectIdsToTexturesRecord.ContainsKey(obj1Id));
            Assert.AreEqual(false, objectsTexturesRecord.objectIdsToTexturesRecord.ContainsKey(obj2Id));
            Assert.AreEqual(0, objectsTexturesRecord.texturesToObjectIdsRecord.Count);
        }

    }
}