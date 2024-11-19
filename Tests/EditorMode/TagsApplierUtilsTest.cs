using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.editor;
using ReupVirtualTwin.models;
using ReupVirtualTwin.webRequestersInterfaces;
using UnityEngine;
using ReupVirtualTwinTests.utils;


namespace ReupVirtualTwinTests.editor
{
    public class TagsApplierUtilsTest
    {
        GameObject building;
        List<Tag> mockApiTags;
        MockTagsApiConsumer mockTagsApiConsumer;

        private GameObject AddChildrenWithTags(GameObject parent, int childrenAmount)
        {
            parent.AddComponent<ObjectTags>();
            for (int i = 0; i < childrenAmount; i++)
            {
                GameObject child = new GameObject($"TAG_{i}_tag_name");
                child.AddComponent<ObjectTags>();
                child.transform.parent = parent.transform;
            }
            return parent;
        }

        private GameObject AddChildrenWithoutTags(GameObject parent, int childrenAmount)
        {
            for (int i = 0; i < childrenAmount; i++)
            {
                GameObject child = new GameObject($"child_{i}");
                child.transform.parent = parent.transform;
            }
            return parent;
        }

        // private static List<Tag> CreateMockTags(int childAmount)
        // {
        //     List<Tag> tags = new List<Tag>();
        //     for (int i = 0; i < childAmount; i++)
        //     {
        //         tags.Add(new Tag()
        //         {
        //             id = $"{i}",
        //             name = $"tag-{i}",
        //             description = $"tag-{i}",
        //             priority = i
        //         });
        //     }
        //     return tags;
        // }

        class MockTagsApiConsumer : ITagsApiConsumer
        {
            public List<Tag> mockApiTags;

            public bool throwException = false;
            public Task<PaginationResult<Tag>> GetTags()
            {
                throw new System.NotImplementedException();
            }

            public Task<PaginationResult<Tag>> GetTags(int page)
            {
                throw new System.NotImplementedException();
            }

            public Task<PaginationResult<Tag>> GetTags(int page, int pageSize)
            {
                if (throwException)
                {
                    throw new System.Exception("Error fetching tags");
                }
                PaginationResult<Tag> requestResult = new PaginationResult<Tag>()
                {
                    count = mockApiTags.Count,
                    next = null,
                    previous = null,
                    results = mockApiTags.ToArray()
                };
                return Task.FromResult(requestResult);
            }
        }

        [SetUp]
        public void SetUp()
        {
            building = new GameObject("building");
            mockApiTags = TagFactory.CreateBulk(5, TagFactory.CreateFaker());
            mockTagsApiConsumer = new MockTagsApiConsumer() { mockApiTags = mockApiTags };
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(building);
            mockApiTags.Clear();
            mockTagsApiConsumer = null;
        }

        [Test]
        public async Task ShouldNotApplyTagsIfGameObjectsHaveNoTagsInTheName()
        {
            AddChildrenWithoutTags(building, 5);

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            for (int i = 0; i < 5; i++)
            {
                GameObject child = building.transform.GetChild(i).gameObject;
                ObjectTags objectTags = child.GetComponent<ObjectTags>();
                Assert.AreEqual(0, objectTags.GetTags().Count);
            }
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ShouldApplyTagsToAllChildren()
        {
            AddChildrenWithTags(building, 5);

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            for (int i = 0; i < 5; i++)
            {
                GameObject child = building.transform.GetChild(i).gameObject;
                ObjectTags objectTags = child.GetComponent<ObjectTags>();
                Assert.IsNotNull(objectTags);
                var tags = objectTags.GetTags();
                Assert.AreEqual(1, tags.Count);
                Assert.AreEqual(mockApiTags[i].id, tags[0].id);
            }
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ShouldApplyTagsToNestedObjects()
        {
            building = AddChildrenWithTags(building, 1);
            GameObject child = building.transform.GetChild(0).gameObject;
            AddChildrenWithTags(child, 1);
            GameObject grandChild = child.transform.GetChild(0).gameObject;

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            ObjectTags objectTagsChild = child.GetComponent<ObjectTags>();
            ObjectTags objectTagsGrandChild = grandChild.GetComponent<ObjectTags>();

            Assert.IsNotNull(objectTagsChild);
            Assert.IsNotNull(objectTagsGrandChild);
            Assert.AreEqual(1, objectTagsChild.GetTags().Count);
            Assert.AreEqual(1, objectTagsGrandChild.GetTags().Count);
            Assert.AreEqual(mockApiTags[0].id, objectTagsChild.GetTags()[0].id);
            Assert.AreEqual(mockApiTags[0].id, objectTagsGrandChild.GetTags()[0].id);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ShouldApplyTagsOnlyToObjectsWhoseNamesContainTagsInsideNestedObjects()
        {
            building = AddChildrenWithoutTags(building, 1);
            GameObject child = building.transform.GetChild(0).gameObject;
            AddChildrenWithTags(child, 1);
            GameObject grandChild = child.transform.GetChild(0).gameObject;

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            ObjectTags objectTagsChild = child.GetComponent<ObjectTags>();
            ObjectTags objectTagsGrandChild = grandChild.GetComponent<ObjectTags>();

            Assert.AreEqual(0, objectTagsChild.GetTags().Count);
            Assert.AreEqual(1, objectTagsGrandChild.GetTags().Count);
            Assert.AreEqual(mockApiTags[0].id, objectTagsGrandChild.GetTags()[0].id);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ShouldNotDuplicateExistingTagsOnObject()
        {
            AddChildrenWithTags(building, 1);
            GameObject child = building.transform.GetChild(0).gameObject;
            child.GetComponent<ObjectTags>().AddTag(mockApiTags[0]);

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            ObjectTags objectTagsChild = child.GetComponent<ObjectTags>();
            Assert.AreEqual(1, objectTagsChild.GetTags().Count);
            Assert.AreEqual(mockApiTags[0].id, objectTagsChild.GetTags()[0].id);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ShouldKeepPreviousAppliedTags()
        {
            AddChildrenWithTags(building, 1);
            GameObject child = building.transform.GetChild(0).gameObject;
            child.GetComponent<ObjectTags>().AddTag(mockApiTags[1]);

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            ObjectTags objectTagsChild = child.GetComponent<ObjectTags>();
            Assert.AreEqual(2, objectTagsChild.GetTags().Count);
            Assert.AreEqual(mockApiTags[1].id, objectTagsChild.GetTags()[0].id);
            Assert.AreEqual(mockApiTags[0].id, objectTagsChild.GetTags()[1].id);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task ShouldReturnErrorMessagesIfTagsAreNotFound()
        {
            AddChildrenWithTags(building, 1);
            mockApiTags.Clear();

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual($"Tags Ids not found in 'TAG_0_tag_name': 0", result[0]);
        }

        [Test]
        public async Task ShouldReturnErrorMessagesIfTagsApiConsumerThrowsException()
        {
            AddChildrenWithTags(building, 1);
            mockTagsApiConsumer.throwException = true;

            List<string> result = await TagsApplierUtil.ApplyTags(building, mockTagsApiConsumer);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Error fetching tags", result[0]);
        }


    }
}