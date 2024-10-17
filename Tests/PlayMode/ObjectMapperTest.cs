using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.dataModels;
using System.Linq;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwinTests.utils;

public class ObjectMapperTest : MonoBehaviour
{
    IObjectMapper objectMapper;
    GameObject mockBuilding;

    [SetUp]
    public void SetUp()
    {
        objectMapper = new ObjectMapper(new TagsController(), new IdController());
        mockBuilding = StubObjectTreeCreator.CreateMockBuilding();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(mockBuilding);
    }

    [UnityTest]
    public IEnumerator ShouldMapOneObject()
    {
        ObjectDTO objectDTO = objectMapper.MapObjectToDTO(mockBuilding);
        Assert.AreEqual(StubObjectTreeCreator.parentId, objectDTO.id);
        Assert.AreEqual(StubObjectTreeCreator.parentTags, objectDTO.tags);
        Assert.IsNull(objectDTO.children);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldMapSeveralObject()
    {
        ObjectDTO[] objectDTOs = objectMapper.MapObjectsToDTO(new GameObject[]
        {
            mockBuilding, mockBuilding.transform.GetChild(0).gameObject, mockBuilding.transform.GetChild(1).gameObject
        }.ToList());
        Assert.AreEqual(3, objectDTOs.Length);
        Assert.AreEqual(StubObjectTreeCreator.parentId, objectDTOs[0].id);
        Assert.AreEqual(StubObjectTreeCreator.parentTags, objectDTOs[0].tags);
        Assert.AreEqual(StubObjectTreeCreator.child0Id, objectDTOs[1].id);
        Assert.AreEqual(StubObjectTreeCreator.child0Tags, objectDTOs[1].tags);
        Assert.AreEqual(StubObjectTreeCreator.child1Id, objectDTOs[2].id);
        Assert.AreEqual(StubObjectTreeCreator.child1Tags, objectDTOs[2].tags);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldMapObjectTree()
    {
        ObjectDTO treeDTO = objectMapper.MapObjectTree(mockBuilding);
        Assert.AreEqual(StubObjectTreeCreator.parentId, treeDTO.id);
        Assert.AreEqual(StubObjectTreeCreator.parentTags, treeDTO.tags);
        Assert.AreEqual(StubObjectTreeCreator.child0Tags, treeDTO.children[0].tags);
        Assert.AreEqual(StubObjectTreeCreator.child0Id, treeDTO.children[0].id);
        Assert.AreEqual(StubObjectTreeCreator.child1Tags, treeDTO.children[1].tags);
        Assert.AreEqual(StubObjectTreeCreator.child1Id, treeDTO.children[1].id);
        Assert.AreEqual(StubObjectTreeCreator.grandChild0Tags, treeDTO.children[0].children[0].tags);
        Assert.AreEqual(StubObjectTreeCreator.grandChild0Id, treeDTO.children[0].children[0].id);

        yield return null;
    }

}
