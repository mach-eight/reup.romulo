using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Threading.Tasks;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managers;
using ReupVirtualTwinTests.mocks;

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
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[0].name, initialTags[0].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[1].name, initialTags[1].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[2].name, initialTags[2].name);
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
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[0].name, initialTags[0].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[1].name, initialTags[1].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[2].name, initialTags[2].name);
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
    }

    [Test]
    public async Task GetSecondLoadOfTags()
    {
        tagsApiManager.tagsApiConsumer = tagsWebRequesterSpy;
        List<Tag> initialTags = await tagsApiManager.GetTags();
        Assert.AreEqual(3, initialTags.Count);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[0].name, initialTags[0].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[1].name, initialTags[1].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[2].name, initialTags[2].name);
        Assert.AreEqual(1, tagsWebRequesterSpy.lastPageRequested);
        Assert.AreEqual(1, tagsWebRequesterSpy.timesFetched);
        List<Tag> moreTags = await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(6, moreTags.Count);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[0].name, moreTags[0].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[1].name, moreTags[1].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[2].name, moreTags[2].name);
        Assert.AreEqual(tagsWebRequesterSpy.secondPage.results[0].name, moreTags[3].name);
        Assert.AreEqual(tagsWebRequesterSpy.secondPage.results[1].name, moreTags[4].name);
        Assert.AreEqual(tagsWebRequesterSpy.secondPage.results[2].name, moreTags[5].name);
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
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[0].name, moreTags[0].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[1].name, moreTags[1].name);
        Assert.AreEqual(tagsWebRequesterSpy.firstPage.results[2].name, moreTags[2].name);
        Assert.AreEqual(tagsWebRequesterSpy.secondPage.results[0].name, moreTags[3].name);
        Assert.AreEqual(tagsWebRequesterSpy.secondPage.results[1].name, moreTags[4].name);
        Assert.AreEqual(tagsWebRequesterSpy.secondPage.results[2].name, moreTags[5].name);
        Assert.AreEqual(tagsWebRequesterSpy.thirdPage.results[0].name, moreTags[6].name);
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

        _ = tagsApiManager.LoadMoreTags();
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(2, delayTagsWebRequesterSpy.numberOfTimesFetched);
        Assert.AreEqual(2, delayTagsWebRequesterSpy.lastPageFetched);

        await Task.Delay(1100); // Wait for the previous request to finish

        _ = tagsApiManager.LoadMoreTags(); // Should fetch
        await Task.Delay(100);
        _ = tagsApiManager.LoadMoreTags(); // Should not fetch
        await Task.Delay(100);
        _ = tagsApiManager.LoadMoreTags(); // Should not fetch
        await Task.Delay(1000);
        _ = tagsApiManager.LoadMoreTags(); // Should fetch

        Assert.AreEqual(4, delayTagsWebRequesterSpy.numberOfTimesFetched);
        Assert.AreEqual(4, delayTagsWebRequesterSpy.lastPageFetched);
    }

    [Test]
    public async Task ShouldNotIncreasePageWhenFailingFetch()
    {
        FailingTagsWebRequesterSpy failingTagsWebRequesterSpy = new FailingTagsWebRequesterSpy();
        tagsApiManager.tagsApiConsumer = failingTagsWebRequesterSpy;
        Assert.AreEqual(0, tagsApiManager.GetCurrentPage());
        await tagsApiManager.LoadMoreTags();
        Assert.AreEqual(0, tagsApiManager.GetCurrentPage());
    }

}

