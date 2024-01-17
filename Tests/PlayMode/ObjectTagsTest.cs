using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.enums;


public class ObjectTagsTest : MonoBehaviour
{
    GameObject containerGameObject;
    ObjectTags objectTags;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject("container");
        objectTags = containerGameObject.AddComponent<ObjectTags>();
    }
    [TearDown]
    public void TearDown()
    {
        Destroy(containerGameObject);
    }
    [UnityTest]
    public IEnumerator ShouldInitializeWithEmptyTagsList()
    {
        Assert.IsEmpty(objectTags.GetTags());
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAddOneTag()
    {
        objectTags.AddTag(ObjectTag.SELECTEABLE);
        Assert.AreEqual(1, objectTags.GetTags().Count);
        Assert.IsTrue(objectTags.GetTags().Contains(ObjectTag.SELECTEABLE));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldRemoveOneTag()
    {
        objectTags.AddTag(ObjectTag.SELECTEABLE);
        Assert.AreEqual(1, objectTags.GetTags().Count);
        Assert.IsTrue(objectTags.GetTags().Contains(ObjectTag.SELECTEABLE));
        yield return null;
        objectTags.RemoveTag(ObjectTag.SELECTEABLE);
        Assert.AreEqual(0, objectTags.GetTags().Count);
        Assert.IsFalse(objectTags.GetTags().Contains(ObjectTag.SELECTEABLE));
        yield return null;
    }
}