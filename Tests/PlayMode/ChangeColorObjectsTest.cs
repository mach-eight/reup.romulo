using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using System;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;

public class ChangeColorObjectsTest : MonoBehaviour
{
    GameObject containerGameObject;
    GameObject changeColorWrapper;
    ChangeColorManager changeColorManager;
    MockMediator mockMediator;
    MockRegistry mockRegistry;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject("containerGameObject");
        changeColorWrapper = new GameObject("changeColorWrapper");
        changeColorManager = containerGameObject.AddComponent<ChangeColorManager>();
        mockMediator = new MockMediator();
        mockRegistry = new MockRegistry();
        changeColorManager.mediator = mockMediator;
        changeColorManager.registry = mockRegistry;
    }
    public List<string> GetIDsArray(List<GameObject> gameObjects)
    {
        List<string> stringIDs = new List<string>();
        foreach (GameObject obj in gameObjects)
        {
            stringIDs.Add(obj.GetComponent<UniqueId>().getId());
        }
        return stringIDs;
    }

    [UnityTest]
    public IEnumerator ShouldReturnArrayWithObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>() { mockRegistry.parentObjects[0], mockRegistry.parentObjects[1] };
        List<string> stringIDs = GetIDsArray(gameObjects);
        Assert.IsNotEmpty(changeColorManager.GetObjectsToChangeColor(stringIDs));
        yield return null;

    }

    [UnityTest]
    public IEnumerator ShouldReturnArrayWithObjectsAndChildren()
    {
        List<GameObject> gameObjects = new List<GameObject>() { mockRegistry.parentObjects[0], mockRegistry.parentObjects[1] };
        List<string> stringIDs = GetIDsArray(gameObjects);
        List<GameObject> objectsToChangeColor = changeColorManager.GetObjectsToChangeColor(stringIDs);
        int expectedCount = gameObjects.Count + gameObjects.Sum(obj => obj.transform.childCount);
        Assert.AreEqual(expectedCount, objectsToChangeColor.Count);
        yield return null;

    }

    [UnityTest]
    public IEnumerator ShouldReturnEmptyListWhenNull()
    {
        List<string> nullArray = null;
        Assert.IsEmpty(changeColorManager.GetObjectsToChangeColor(nullArray));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldReturnEmptyListWhenEmpty()
    {
        List<string> emptyArray = new();
        Assert.IsEmpty(changeColorManager.GetObjectsToChangeColor(emptyArray));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldParseStringToColor()
    {
        Color? parsedColor = Utils.ParseColor("#0000FF");
        Assert.IsInstanceOf<Color>(parsedColor);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldFailWhenStringIsNotHex()
    {
        Color? parsedColor = Utils.ParseColor("NotAnHex");
        Assert.IsNull(parsedColor);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldChangeColorObjectsAndChildren()
    {
        List<GameObject> gameObjects = new List<GameObject>() { mockRegistry.parentObjects[0], mockRegistry.parentObjects[1] };
        List<string> stringIDs = GetIDsArray(gameObjects);
        List<GameObject> objectsToChangeColor = changeColorManager.GetObjectsToChangeColor(stringIDs);
        changeColorManager.ChangeObjectsColor(objectsToChangeColor, Color.blue);

        yield return null;

        Renderer renderer0 = mockRegistry.parent0.GetComponent<Renderer>();
        Assert.AreEqual(renderer0.material.color, Color.blue);

        Renderer renderer1 = mockRegistry.parent1.GetComponent<Renderer>();
        Assert.AreEqual(renderer1.material.color, Color.blue);

        Renderer renderer2 = mockRegistry.child0.GetComponent<Renderer>();
        Assert.AreEqual(renderer2.material.color, Color.blue);

        Renderer renderer3 = mockRegistry.child1.GetComponent<Renderer>();
        Assert.AreEqual(renderer3.material.color, Color.blue);
    }

    private class MockMediator : IMediator
    {
        public bool deleteModeActive = false;
        public bool notified = false;

        public void Notify(ReupEvent eventName)
        {
            if (eventName == ReupEvent.objectsDeleted)
            {
                notified = true;
            }
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            throw new System.NotImplementedException();
        }
    }
    public class MockRegistry : IRegistry
    {
        public GameObject parent0 = new GameObject();
        public GameObject parent1 = new GameObject();
        public GameObject child0 = new GameObject();
        public GameObject child1 = new GameObject();
        public List<GameObject> parentObjects = new List<GameObject>();
        public MockRegistry()
        {
            parent0.AddComponent<UniqueId>().GenerateId();
            parent0.AddComponent<MeshRenderer>();

            parent1.AddComponent<UniqueId>().GenerateId();
            parent1.AddComponent<MeshRenderer>();

            child0.AddComponent<UniqueId>().GenerateId();
            child0.AddComponent<MeshRenderer>();
            child0.transform.parent = parent0.transform;

            child1.AddComponent<UniqueId>().GenerateId();
            child1.AddComponent<MeshRenderer>();
            child1.transform.parent = parent1.transform;

            parentObjects.Add(parent0);
            parentObjects.Add(parent1);
        }

        public void RemoveItem(GameObject item)
        {
            throw new System.NotImplementedException();
        }
        public int GetItemCount()
        {
            throw new System.NotImplementedException();
        }
        public void ClearRegistry()
        {
            throw new System.NotImplementedException();
        }
        public void AddItem(GameObject obj)
        {
            throw new NotImplementedException();
        }
        public GameObject GetItemWithGuid(string guid)
        {
            foreach (GameObject obj in parentObjects)
            {
                if (obj == null) continue;
                var uniqueIdentifier = obj.GetComponent<UniqueId>();
                if (uniqueIdentifier.isIdCorrect(guid))
                {
                    return obj;
                }
            }
            return null;
        }
        public List<GameObject> GetItemsWithGuids(string[] guids)
        {
            var foundObjects = new List<GameObject>();
            foreach (string guid in guids)
            {
                foundObjects.Add(GetItemWithGuid(guid));
            }
            return foundObjects;
        }

        public List<GameObject> GetItemTreesWithParentGuids(List<string> stringIDs)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            List<GameObject> allGameObjectsToEdit = new List<GameObject>();
            if (stringIDs != null && stringIDs.Count != 0)
            {
                gameObjects = GetItemsWithGuids(stringIDs.ToArray());
                gameObjects.RemoveAll(obj => obj == null);
                allGameObjectsToEdit.AddRange(gameObjects);
                foreach (GameObject obj in gameObjects)
                {
                    AddChildrenToList(obj.transform, allGameObjectsToEdit);
                }

            }
            return allGameObjectsToEdit;
        }
        private void AddChildrenToList(Transform parent, List<GameObject> list)
        {
            foreach (Transform childTransform in parent)
            {
                if (childTransform.gameObject != null)
                {
                    list.Add(childTransform.gameObject);
                    AddChildrenToList(childTransform, list);
                }
            }
        }
    }
}
