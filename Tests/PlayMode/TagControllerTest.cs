using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwinTests.utils;

public class tagsControllerTest : MonoBehaviour
{
    GameObject taggedObject0;
    GameObject taggedObject1;
    GameObject nonTaggedObject0;
    TagsController tagsController;
    ObjectTags objectTags0;
    ObjectTags objectTags1;
    List<Tag> tags = TagFactory.CreateBulk(3);

    [SetUp]
    public void SetUp()
    {
        tagsController = new TagsController();
        taggedObject0 = new GameObject("taggedObj0");
        objectTags0 = taggedObject0.AddComponent<ObjectTags>();
        objectTags0.AddTags(new Tag[2] {tags[0], tags[1]});
        taggedObject1 = new GameObject("taggedObject1");
        objectTags1 = taggedObject1.AddComponent<ObjectTags>();
        objectTags1.AddTag(tags[0]);
        nonTaggedObject0 = new GameObject("nonTaggedObj0");
    }
    [TearDown]
    public void TearDown()
    {
        Destroy(taggedObject0);
        Destroy(taggedObject1);
        Destroy(nonTaggedObject0);
    }
    [UnityTest]
    public IEnumerator ShouldReturnFalseOnCheckForTags()
    {
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject0, tags[2].id));
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject1, tags[2].id));
        Assert.IsFalse(tagsController.DoesObjectHaveTag(nonTaggedObject0, tags[2].id));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldReturnTrueOnCheckForTags()
    {
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject0, tags[0].id));
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject1, tags[0].id));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAddTags()
    {
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject0, tags[2].id));
        tagsController.AddTagToObject(taggedObject0, tags[2]);
        yield return null;
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject0, tags[2].id));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldRemoveTags()
    {
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject0, tags[0].id));
        tagsController.RemoveTagFromObject(taggedObject0, tags[0]);
        yield return null;
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject0, tags[0].id));
        yield return null;
    }

}
