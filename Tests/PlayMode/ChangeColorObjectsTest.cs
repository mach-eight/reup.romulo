using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.models;
using NUnit.Framework;

public class ChangeColorObjectsTest : MonoBehaviour
{
    GameObject containerGameObject;
    ChangeColorManager changeColorManager;
    MockMediator mockMediator;

    GameObject meshedParent;
    GameObject unmeshedParent;
    GameObject unmeshedChild;
    GameObject meshedChild;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject("containerGameObject");
        changeColorManager = containerGameObject.AddComponent<ChangeColorManager>();
        mockMediator = new MockMediator();
        changeColorManager.mediator = mockMediator;
        CreateObjects();
    }
    [TearDown]
    public void TearDown()
    {
        Destroy(containerGameObject);
        Destroy(meshedParent);
        Destroy(unmeshedParent);
        Destroy(meshedChild);
        Destroy(unmeshedChild);
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
    public IEnumerator ShouldNotComplain_When_TryingToChangeColorOfUnmeshedObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>() { unmeshedParent, unmeshedChild };
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);
        yield return null;
    }


    [UnityTest]
    public IEnumerator ShouldChangeColorObjects()
    {
        List<GameObject> gameObjects = new() { meshedParent, meshedChild, unmeshedParent, unmeshedChild};
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);

        yield return null;

        Assert.AreEqual(Color.blue, meshedParent.GetComponent<Renderer>().material.color);
        Assert.AreEqual(Color.blue, meshedChild.GetComponent<Renderer>().material.color);

        yield return null;
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

    private void CreateObjects()
    {
        meshedParent = new GameObject();
        unmeshedParent = new GameObject();
        unmeshedChild = new GameObject();
        meshedChild = new GameObject();

        meshedParent.AddComponent<UniqueId>().GenerateId();
        meshedParent.AddComponent<MeshRenderer>();

        unmeshedParent.AddComponent<UniqueId>().GenerateId();

        unmeshedChild.AddComponent<UniqueId>().GenerateId();
        unmeshedChild.transform.parent = meshedParent.transform;

        meshedChild.AddComponent<UniqueId>().GenerateId();
        meshedChild.AddComponent<MeshRenderer>();
        meshedChild.transform.parent = unmeshedParent.transform;
    }
}
