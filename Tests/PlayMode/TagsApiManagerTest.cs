using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Threading.Tasks;
using Tests.PlayMode.Mocks;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managers;

public class TagsApiManagerTest : MonoBehaviour
{
    GameObject tagsApiManagerGameObject;
    TagsApiManager tagsApiManager;
    TagsWebRequesterSpy tagsWebRequesterSpy;

    [SetUp]
    public void SetUp()
    {
        tagsApiManagerGameObject = new GameObject("TagApiManagerGameObject");
        tagsApiManager = tagsApiManagerGameObject.AddComponent<TagsApiManager>();
        tagsWebRequesterSpy = new TagsWebRequesterSpy();
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(tagsApiManagerGameObject);
    }

    [Test]
    public async Task GetInitialLoadOfTags()
    {
        tagsApiManager.tagsApiConsumer = tagsWebRequesterSpy;
        List<Tag> initialTags = await tagsApiManager.GetTags();
        Assert.AreEqual(3, initialTags.Count);
        Assert.AreEqual("tag0", initialTags[0].name);
        Assert.AreEqual("tag1", initialTags[1].name);
        Assert.AreEqual("tag2", initialTags[2].name);
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
    }

    [Test]
    public async Task ShouldGetAlreadyFetchedTagsWithoutFetchingMore()
    {
        tagsApiManager.tagsApiConsumer = tagsWebRequesterSpy;
        await tagsApiManager.GetTags();
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        List<Tag> initialTags = await tagsApiManager.GetTags();
        Assert.AreEqual(3, initialTags.Count);
        Assert.AreEqual("tag0", initialTags[0].name);
        Assert.AreEqual("tag1", initialTags[1].name);
        Assert.AreEqual("tag2", initialTags[2].name);
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
    }

    [Test]
    public async Task GetSecondLoadOfTags()
    {
        tagsApiManager.tagsApiConsumer = tagsWebRequesterSpy;
        List<Tag> initialTags = await tagsApiManager.GetTags();
        Assert.AreEqual(3, initialTags.Count);
        Assert.AreEqual("tag0", initialTags[0].name);
        Assert.AreEqual("tag1", initialTags[1].name);
        Assert.AreEqual("tag2", initialTags[2].name);
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
        List<Tag> moreTags = await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(6, moreTags.Count);
        Assert.AreEqual("tag0", moreTags[0].name);
        Assert.AreEqual("tag1", moreTags[1].name);
        Assert.AreEqual("tag2", moreTags[2].name);
        Assert.AreEqual("tag3", moreTags[3].name);
        Assert.AreEqual("tag4", moreTags[4].name);
        Assert.AreEqual("tag5", moreTags[5].name);
        Assert.AreEqual(2, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(2, tagsWebRequesterSpy.timesFetched);
    }

    [Test]
    public async Task GetThirdLoadOfTags()
    {
        tagsApiManager.tagsApiConsumer = tagsWebRequesterSpy;
        await tagsApiManager.GetTags();
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(2, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(2, tagsWebRequesterSpy.timesFetched);
        List<Tag> moreTags = await tagsApiManager.LoadMoreTags();
        Assert.AreEqual("tag0", moreTags[0].name);
        Assert.AreEqual("tag1", moreTags[1].name);
        Assert.AreEqual("tag2", moreTags[2].name);
        Assert.AreEqual("tag3", moreTags[3].name);
        Assert.AreEqual("tag4", moreTags[4].name);
        Assert.AreEqual("tag5", moreTags[5].name);
        Assert.AreEqual("tag6", moreTags[6].name);
        Assert.AreEqual("tag7", moreTags[7].name);
        Assert.AreEqual(8, moreTags.Count);
        Assert.AreEqual(3, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(3, tagsWebRequesterSpy.timesFetched);
    }
    [Test]
    public async Task ShouldNotFetchApiIfNoMoreTagsAreAvailable()
    {
        tagsApiManager.tagsApiConsumer = tagsWebRequesterSpy;
        await tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(3, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(3, tagsWebRequesterSpy.timesFetched);
        await tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(3, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(3, tagsWebRequesterSpy.timesFetched);
    }
    [Test]
    public async Task ShouldNotFetchMoreTagsIfNextIsEmptyString()
    {
        EmptyStringsTagsWebRequesterSpy emptyStringWebRequesterSpy = new EmptyStringsTagsWebRequesterSpy();
        tagsApiManager.tagsApiConsumer = emptyStringWebRequesterSpy;
        await tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(1, emptyStringWebRequesterSpy.numberOfTimesFetched);
    }

    [Test]
    public async Task ShouldNotFetchIfPreviousResponseIsStillWaiting()
    {
        DelayTagsWebRequesterSpy delayTagsWebRequesterSpy = new DelayTagsWebRequesterSpy(1000);
        tagsApiManager.tagsApiConsumer = delayTagsWebRequesterSpy;

        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(1, delayTagsWebRequesterSpy.numberOfTimesFetched);
        Assert.AreEqual(1, delayTagsWebRequesterSpy.lastPageFetched);

        tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(2, delayTagsWebRequesterSpy.numberOfTimesFetched);
        Assert.AreEqual(2, delayTagsWebRequesterSpy.lastPageFetched);

        await Task.Delay(1100); // Wait for the previous request to finish

        tagsApiManager.LoadMoreTags(); // Should fetch
        await Task.Delay(100);
        tagsApiManager.LoadMoreTags(); // Should not fetch
        await Task.Delay(100);
        tagsApiManager.LoadMoreTags(); // Should not fetch
        await Task.Delay(1000);
        tagsApiManager.LoadMoreTags(); // Should fetch

        Assert.AreEqual(4, delayTagsWebRequesterSpy.numberOfTimesFetched);
        Assert.AreEqual(4, delayTagsWebRequesterSpy.lastPageFetched);
    }

}

