using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.controllerInterfaces;
using System.Linq;

public class ObjectMapperTest : MonoBehaviour
{
    ObjectMapper objectMapper;
    MockTagsController mockTagsController;
    MockIdController mockIdController;

    [SetUp]
    public void SetUp()
    {
        mockTagsController = new MockTagsController();
        mockIdController = new MockIdController();
        objectMapper = new ObjectMapper(mockTagsController, mockIdController);
    }

    [UnityTest]
    public IEnumerator ShouldMapOneObject()
    {
        ObjectDTO objectDTO = objectMapper.MapObjectToDTO(new GameObject("obj"));
        Assert.AreEqual(mockIdController.id, objectDTO.id);
        Assert.AreEqual(mockTagsController.tags, objectDTO.tags);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldMapSeveralObject()
    {
        var obj0 = new GameObject("obj0");
        var obj1 = new GameObject("obj1");
        var obj2 = new GameObject("obj2");
        ObjectDTO[] objectDTOs = objectMapper.MapObjectsToDTO(new GameObject[]
        {
            obj0, obj1, obj2
        }.ToList());
        Assert.AreEqual(3, objectDTOs.Length);
        Assert.AreEqual(objectMapper.MapObjectToDTO(obj0).id, objectDTOs[0].id);
        Assert.AreEqual(objectMapper.MapObjectToDTO(obj0).tags, objectDTOs[0].tags);
        Assert.AreEqual(objectMapper.MapObjectToDTO(obj1).id, objectDTOs[1].id);
        Assert.AreEqual(objectMapper.MapObjectToDTO(obj1).tags, objectDTOs[1].tags);
        Assert.AreEqual(objectMapper.MapObjectToDTO(obj2).id, objectDTOs[2].id);
        Assert.AreEqual(objectMapper.MapObjectToDTO(obj2).tags, objectDTOs[2].tags);
        yield return null;
    }

    private class MockTagsController : ITagsController
    {
        public string[] tags = new string[2] { "tag0", "tag1" };
        public List<string> AddTagToObject(GameObject obj, string tag)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesObjectHaveTag(GameObject obj, string tag)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetTagNamesFromObject(GameObject obj)
        {
            return tags;
        }

        public List<string> GetTagsFromObject(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        public List<string> RemoveTagFromOjbect(GameObject obj, string tag)
        {
            throw new System.NotImplementedException();
        }
    }
    private class MockIdController : IIdGetterController
    {
        public string id = "the obj id";
        public string GetIdFromObject(GameObject obj)
        {
            return id;
        }
    }
}
