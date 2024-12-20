using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwinTests.utils;


public class ObjectTagsTest : MonoBehaviour
{
    GameObject containerGameObject;
    ObjectTags objectTags;
    Tag testTag;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject("container");
        objectTags = containerGameObject.AddComponent<ObjectTags>();
        testTag = TagFactory.Create();
    }
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(containerGameObject);
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
        objectTags.AddTag(testTag);
        Assert.AreEqual(1, objectTags.GetTags().Count);
        Assert.IsTrue(objectTags.GetTags().Contains(testTag));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldRemoveOneTag()
    {
        objectTags.AddTag(testTag);
        Assert.AreEqual(1, objectTags.GetTags().Count);
        Assert.IsTrue(objectTags.GetTags().Contains(testTag));
        yield return null;
        objectTags.RemoveTag(testTag);
        Assert.AreEqual(0, objectTags.GetTags().Count);
        Assert.IsFalse(objectTags.GetTags().Contains(testTag));
        yield return null;
    }
}
