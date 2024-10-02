using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.controllers
{
    public class BuildingVisibilityControllerTest
    {
        IBuildingVisibilityController buildingVisibilityController;
        MockObjectRegistry objectRegistrySpy;
        GameObject mockBuildingObject;
        GameObject child0;
        GameObject child1;
        GameObject child2;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            CreteMockBuildingObject();
            List<GameObject> objects = new List<GameObject> { child0, child1, child2 };
            objectRegistrySpy = new MockObjectRegistry(objects);
            yield return null;
            buildingVisibilityController = new BuildingVisibilityController(objectRegistrySpy, mockBuildingObject);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (mockBuildingObject != null)
            {
                Object.Destroy(mockBuildingObject);
            }
            Object.Destroy(child0);
            Object.Destroy(child1);
            Object.Destroy(child2);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldCreateController()
        {
            Assert.IsNotNull(buildingVisibilityController);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SetObjectsVisibility_WhenNoObjectIdsProvided_ReturnsFailure()
        {
            string[] objectsIds = new string[] { };

            TaskResult result = buildingVisibilityController.SetObjectsVisibility(objectsIds, true);

            Assert.AreEqual(result.isSuccess, false);
            Assert.AreEqual(result.error, "Hide/Show objects failed: No object IDs provided");
            yield return null;
        }

        [UnityTest]
        public IEnumerator SetObjectsVisibility_WhenObjectIdsProvided_ReturnsSuccess()
        {
            string[] objectsIds = objectRegistrySpy.GetObjectListIds();
            DeactivateAllChildren();

            TaskResult result = buildingVisibilityController.SetObjectsVisibility(objectsIds, true);

            Assert.AreEqual(result.isSuccess, true);
            Assert.AreEqual(child0.activeSelf, true);
            Assert.AreEqual(child1.activeSelf, true);
            Assert.AreEqual(child2.activeSelf, true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SetObjectsVisibility_WhenObjectIdsProvidedAndObjectNotFound_ReturnsFailure()
        {
            string[] objectsIds = new string[] { "id-0" };

            TaskResult result = buildingVisibilityController.SetObjectsVisibility(objectsIds, true);

            Assert.AreEqual(result.isSuccess, false);
            Assert.AreEqual(result.error, "Hide/Show objects failed: The following object Ids were not found: id-0");
            yield return null;
        }

        [UnityTest]
        public IEnumerator SetObjectsVisibility_WhenObjectIdsProvidedAndObjectNotFound_ReturnsFailureWithMultipleIds()
        {
            string[] objectsIds = new string[] { "id-0", "id-1", "id-2" };

            TaskResult result = buildingVisibilityController.SetObjectsVisibility(objectsIds, true);

            Assert.AreEqual(result.isSuccess, false);
            Assert.AreEqual(result.error, "Hide/Show objects failed: The following object Ids were not found: id-0, id-1, id-2");
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowAllObjects_WhenBuildingIsSet_ReturnsSuccess()
        {
            DeactivateAllChildren();

            TaskResult result = buildingVisibilityController.ShowAllObjects();
            
            Assert.AreEqual(result.isSuccess, true);
            Assert.AreEqual(child0.activeSelf, true);
            Assert.AreEqual(child1.activeSelf, true);
            Assert.AreEqual(child2.activeSelf, true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShowAllObjects_WhenBuildingIsNull_ReturnsFailure()
        {
            Object.Destroy(mockBuildingObject);
            yield return null;

            TaskResult result = buildingVisibilityController.ShowAllObjects();

            Assert.AreEqual(result.isSuccess, false);
            Assert.AreEqual(result.error, "Show all objects failed: Building object is null");
            yield return null;
        }

        private void CreteMockBuildingObject()
        {
            mockBuildingObject = new GameObject();
            child0 = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();
            child0.transform.parent = mockBuildingObject.transform;
            child1.transform.parent = mockBuildingObject.transform;
            child2.transform.parent = mockBuildingObject.transform;;
        }

        private void DeactivateAllChildren()
        {
            child0.SetActive(false);
            child1.SetActive(false);
            child2.SetActive(false);
        }

    }

    class MockObjectRegistry: IObjectRegistry
    {
        public List<GameObject> objects = new List<GameObject>();
        public string[] lastRequestedObjectIds;
        public List<string[]> requestedObjectIds;
        public MockObjectRegistry(List<GameObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = objects[i];
                obj.AddComponent<UniqueId>().GenerateId();
                this.objects.Add(obj);
            }
            requestedObjectIds = new List<string[]>();
        }
        public void AddObject(GameObject item)
        {
            throw new System.NotImplementedException();
        }

        public void ClearRegistry()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetObjects()
        {
            return objects;
        }

        public int GetObjectsCount()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetObjectsWithGuids(string[] guids)
        {
            var foundObjects = new List<GameObject>();

            foreach (string guid in guids)
            {
                GameObject obj = objects.FirstOrDefault(obj => obj.GetComponent<IUniqueIdentifier>().getId() == guid);
                foundObjects.Add(obj);
            }

            return foundObjects;
        }

        public string[] GetObjectListIds()
        {
            return objects.ConvertAll(obj => obj.GetComponent<IUniqueIdentifier>().getId()).ToArray();
        }

        public GameObject GetObjectWithGuid(string guid)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveObject(string id, GameObject item)
        {
            throw new System.NotImplementedException();
        }
    }
}