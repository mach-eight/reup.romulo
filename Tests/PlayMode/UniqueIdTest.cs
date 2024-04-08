using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;


public class UniqueIdTest : MonoBehaviour
{
    GameObject testObj;
    IUniqueIdentifier uniqueIdentifier;

    [SetUp]
    public void SetUp()
    {
        testObj = new GameObject("testObj");
        uniqueIdentifier = testObj.AddComponent<UniqueId>();
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(testObj);
    }

    [UnityTest]
    public IEnumerator ShouldNotAssignAnEmptyId()
    {
        Assert.That(() => uniqueIdentifier.AssignId(""), Throws.Exception);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldNotAssignANullId()
    {
        Assert.That(() => uniqueIdentifier.AssignId(null), Throws.Exception);
        yield return null;
    }

}
