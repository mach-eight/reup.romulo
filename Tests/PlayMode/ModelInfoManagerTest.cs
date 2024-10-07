using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;

public class ModelInfoManagerTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;
    ObjectMapper objectMapper = new ObjectMapper(new TagsController(), new IdController());
    GameObject buildingGameObject;
    ModelInfoManager modelInfoManager;
    const int BUILDING_CHILDREN_DEPTH = 30;
    List<GameObject> spaceSelectors;


    [SetUp]
    public void SetUp()
    {
        buildingGameObject = StubObjectTreeCreator.CreateMockBuilding(BUILDING_CHILDREN_DEPTH);
        sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingWithBuildingObject(buildingGameObject);
        modelInfoManager = sceneObjects.modelInfoManager;
    }
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        SpaceSelectorFabric.DestroySpaceSelectors(spaceSelectors);
        spaceSelectors?.Clear();
        yield return null;
    }

    private int NumberOfObjectsInTree(ObjectDTO tree)
    {
        int count = 1;
        foreach (ObjectDTO child in tree.children)
        {
            count += NumberOfObjectsInTree(child);
        }
        return count;
    }

    private bool CompareObjectDTOs(ObjectDTO expected, ObjectDTO obtained)
    {
        JObject expectedJson = JObject.FromObject(expected);
        JObject obtainedJson = JObject.FromObject(obtained);
        return JToken.DeepEquals(expectedJson, obtainedJson);
    }

    bool IdStructureAreEqual(JObject expected, JObject obtained)
    {
        JObject expectedIdStructure = ExtractIdStructureOnly(expected);
        JObject obtainedIdStructure = ExtractIdStructureOnly(obtained);
        return JToken.DeepEquals(expectedIdStructure, obtainedIdStructure);
    }

    JObject ExtractIdStructureOnly(JObject obj)
    {
        List<JObject> extractedChildren = new List<JObject>();
        JToken children = obj["children"];
        if (children == null)
        {
            return new JObject(new JProperty("id", obj["id"]));
        }
        foreach (JObject child in children)
        {
            extractedChildren.Add(ExtractIdStructureOnly(child));
        }
        return new JObject(
            new JProperty("id", obj["id"]),
            new JProperty("children", new JArray(extractedChildren.ToArray())));
    }

    [UnityTest]
    public IEnumerator ShouldObtainTheModelInfoMessage()
    {
        WebMessage<JObject> message = modelInfoManager.ObtainModelInfoMessage();
        Assert.IsNotNull(message);
        Assert.AreEqual(WebMessageType.requestModelInfoSuccess, message.type);
        Assert.IsNotNull(message.payload);
        Assert.AreEqual(modelInfoManager.buildVersion, message.payload["buildVersion"].ToString());
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldObtainTheUpdateBuildingMessage()
    {
        WebMessage<JObject> message = modelInfoManager.ObtainUpdateBuildingMessage();
        Assert.IsNotNull(message);
        Assert.AreEqual(WebMessageType.updateBuilding, message.type);
        Assert.IsNotNull(message.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldObtainBuildingTreeDataStructure()
    {
        ObjectDTO expectedBuildingTreeDataStructure = objectMapper.MapObjectTree(buildingGameObject);
        WebMessage<JObject> message = modelInfoManager.ObtainModelInfoMessage();
        ObjectDTO buildingTreeDataStructure = message.payload["building"].ToObject<ObjectDTO>();
        Assert.IsTrue(CompareObjectDTOs(expectedBuildingTreeDataStructure, buildingTreeDataStructure));
        Assert.AreEqual(NumberOfObjectsInTree(expectedBuildingTreeDataStructure), NumberOfObjectsInTree(buildingTreeDataStructure));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldReturnTotalNumberOfObjects()
    {
        int numberOfObjectsByDefaultInStubBuilding = 4;
        int numberOfObjectsInTotal = numberOfObjectsByDefaultInStubBuilding + BUILDING_CHILDREN_DEPTH;
        WebMessage<JObject> message = modelInfoManager.ObtainModelInfoMessage();
        ObjectDTO buildingTreeDataStructure = message.payload["building"].ToObject<ObjectDTO>();
        Assert.AreEqual(numberOfObjectsInTotal, NumberOfObjectsInTree(buildingTreeDataStructure));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldGetSceneStateMessage_with_SameIdStructureAsBuilding()
    {
        JObject sceneSTate = modelInfoManager.GetSceneState();
        ObjectDTO buildingTreeDataStructure = objectMapper.MapObjectTree(buildingGameObject);
        Assert.IsTrue(IdStructureAreEqual(JObject.FromObject(buildingTreeDataStructure), sceneSTate));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldGetSceneStateMessage_with_noAppearanceInfoAtTheBeginning()
    {
        JObject sceneSTate = modelInfoManager.GetSceneState();
        Assert.IsNull(sceneSTate["appearance"]);
        Assert.IsNull(sceneSTate["children"][0]["appearance"]);
        Assert.IsNull(sceneSTate["children"][1]["appearance"]);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldGetColorInfoInSceneStateMessage()
    {
        IObjectMetaDataGetterSetter metaDataComponent = buildingGameObject.GetComponent<IObjectMetaDataGetterSetter>();
        JObject parentMetaData = new JObject
        {
            { "appearance", new JObject
                {
                    { "color", "test-color-parent" }
                }
            }
        };
        metaDataComponent.objectMetaData = parentMetaData;

        IObjectMetaDataGetterSetter metaDataChildComponent = buildingGameObject.transform.GetChild(0).gameObject.GetComponent<IObjectMetaDataGetterSetter>();
        JObject childMetaData = new JObject
        {
            { "appearance", new JObject
                {
                    { "color", "test-color-child" }
                }
            }
        };
        metaDataChildComponent.objectMetaData = childMetaData;

        JObject sceneState = modelInfoManager.GetSceneState();
        Assert.AreEqual(parentMetaData["appearance"]["color"], sceneState["appearance"]["color"]);
        Assert.AreEqual(childMetaData["appearance"]["color"], sceneState["children"][0]["appearance"]["color"]);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldContainNamesInSpaceSelectorsList()
    {
        spaceSelectors = SpaceSelectorFabric.CreateBulk(5);
        yield return null;
        WebMessage<JObject> message = modelInfoManager.ObtainModelInfoMessage();
        JArray spaceSelectorsList = message.payload["spaceSelectors"].ToObject<JArray>();
        Assert.AreEqual(5, spaceSelectorsList.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(spaceSelectors[i].GetComponent<SpaceJumpPoint>().name, spaceSelectorsList[i]["name"].ToString());
        }
        yield return null;

    }

    [UnityTest]
    public IEnumerator ShouldContainIdsInSpaceSelectorsList()
    {
        spaceSelectors = SpaceSelectorFabric.CreateBulk(5);
        yield return null;
        WebMessage<JObject> message = modelInfoManager.ObtainModelInfoMessage();
        JArray spaceSelectorsList = message.payload["spaceSelectors"].ToObject<JArray>();
        Assert.AreEqual(5, spaceSelectorsList.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(spaceSelectors[i].GetComponent<SpaceJumpPoint>().id, spaceSelectorsList[i]["id"].ToString());
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator SpaceSelectorsIdsShouldBeDifferentThanEmptyOrNull()
    {
        int numberOfSpaceSelectors = 5;
        spaceSelectors = SpaceSelectorFabric.CreateBulk(numberOfSpaceSelectors);
        WebMessage<JObject> message = modelInfoManager.ObtainModelInfoMessage();
        JArray spaceSelectorsList = message.payload["spaceSelectors"].ToObject<JArray>();
        Assert.AreEqual(numberOfSpaceSelectors, spaceSelectorsList.Count);
        for (int i = 0; i < 5; i++)
        {
            string id = spaceSelectorsList[i]["id"].ToString().Trim();
            Assert.IsFalse(string.IsNullOrEmpty(id));
        }
        yield return null;
    }

}
