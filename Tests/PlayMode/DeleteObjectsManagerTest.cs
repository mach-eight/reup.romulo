using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Linq;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.dataModels;
using System;
using ReupVirtualTwin.models;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.modelInterfaces;

public class DeleteObjectsManagerTest : MonoBehaviour
{
    GameObject containerGameObject;
    GameObject runtimeDeleteObj;
    GameObject deleteWrapper;
    DeleteObjectsManager deleteObjectsManager;
    MockMediator mockMediator;
    MockRegistry mockRegistry;
    GameObject deletableObject0;
    GameObject deletableObject1;
    GameObject nonDeletableObject;
    public List<GameObject> allObjects = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject("containerGameObject");
        deleteWrapper = new GameObject("deleteWrapper");
        deleteObjectsManager = containerGameObject.AddComponent<DeleteObjectsManager>();
        deleteObjectsManager.tagsController = new TagsController();
        mockMediator = new MockMediator();
        deleteObjectsManager.mediator = mockMediator;
        mockRegistry = new MockRegistry();
        deleteObjectsManager.registry = mockRegistry;
        allObjects = mockRegistry.allObjects;
    }
    public string ListToString(List<string> idsList)
    {
        string idsString = string.Join(",", idsList);
        return idsString;
    }
    public List<string> GetIDsList(List<GameObject> gameObjects)
    {
        List<string> stringIDs = new List<string>();
        foreach (GameObject obj in gameObjects)
        {
            stringIDs.Add(obj.GetComponent<UniqueId>().getId());
        }
        return stringIDs;
    }
    [UnityTest]
    public IEnumerator ShouldDeleteDeletableObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>() { allObjects[0], allObjects[1] };
        string stringIDs = ListToString(GetIDsList(gameObjects));
        Assert.IsNotEmpty(deleteObjectsManager.GetDeletableObjects(stringIDs));
        yield return null;

    }
    [UnityTest]
    public IEnumerator ShouldFailWhenEmptyIDsString()
    {
        Assert.IsEmpty(deleteObjectsManager.GetDeletableObjects(""));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldFailWhenTryingToDeleteNonDeletableObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>() { allObjects[0], allObjects[1], allObjects[2]};
        string stringIDs = ListToString(GetIDsList(gameObjects));
        Assert.IsEmpty(deleteObjectsManager.GetDeletableObjects(stringIDs));
        yield return null;

    }
    private class MockMediator : IMediator
    {
        public bool deleteModeActive = false;
        public bool notified = false;

        public void Notify(ReupEvent eventName)
        {
            if (eventName == ReupEvent.objectsDeleted){
                notified = true;
            }
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            throw new System.NotImplementedException();
        }
    }
    private class MockRegistry : IRegistry
    {
        public List<GameObject> allObjects = new List<GameObject>();
        public MockRegistry()
        {
            GameObject deletableObject0 = new GameObject("deletableObject0");
            deletableObject0.AddComponent<ObjectTags>().AddTags(new ObjectTag[2] { ObjectTag.SELECTABLE, ObjectTag.DELETABLE });
            deletableObject0.AddComponent<UniqueId>().GenerateId();
            GameObject deletableObject1 = new GameObject("deletableObject1");
            deletableObject1.AddComponent<ObjectTags>().AddTags(new ObjectTag[2] { ObjectTag.SELECTABLE, ObjectTag.DELETABLE });
            deletableObject1.AddComponent<UniqueId>().GenerateId();
            GameObject nonDeletableObject = new GameObject("nonDeletableObject");
            nonDeletableObject.AddComponent<ObjectTags>().AddTags(new ObjectTag[1] { ObjectTag.SELECTABLE });
            nonDeletableObject.AddComponent<UniqueId>().GenerateId();
            allObjects.Add(deletableObject0);
            allObjects.Add(deletableObject1);
            allObjects.Add(nonDeletableObject);
        }

        public void AddItem(GameObject obj)
        {
            throw new NotImplementedException();
        }
        public GameObject GetItemWithGuid(string guid)
        {
            foreach (GameObject obj in allObjects)
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

        public void RemoveItem(GameObject item)
        {
            throw new NotImplementedException();
        }

        public int GetItemCount()
        {
            throw new NotImplementedException();
        }

        public void ClearRegistry()
        {
            throw new NotImplementedException();
        }
    }

}
