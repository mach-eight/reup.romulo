using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwin.models;
using UnityEditor;
using Newtonsoft.Json.Linq;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using Newtonsoft.Json;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwinTests.mocks;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwinTests.IOTests
{
    public class ModelInfoRequestTest
    {
        GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/PlatformNoCollider.prefab");
        ReupSceneInstantiator.SceneObjects sceneObjects;
        EditMediator editMediator;
        Transform character;
        WebMessageSenderSpy webMessageSenderSpy;
        GameObject platform;
        List<Tag> platformTags;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab);
            character = sceneObjects.character;
            editMediator = sceneObjects.editMediator;
            webMessageSenderSpy = sceneObjects.webMessageSenderSpy;
            platform = sceneObjects.building;
            SetCharacterOverPlatform();
            SetPlatformTags();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }
        void SetPlatformTags()
        {
            ITagsController tagsController = new TagsController();
            platformTags = TagFactory.CreateBulk(3);
            foreach (Tag tag in platformTags)
            {
                tagsController.AddTagToObject(platform, tag);
            }
        }
        void SetCharacterOverPlatform()
        {
            character.position = platform.transform.position + 10 * Vector3.up;
        }

        [UnityTest]
        public IEnumerator ShouldReturnListOfObjectsTags()
        {
            string requestId = "test-request-id";
            JObject requestMessage = new JObject(
                new JProperty("type", WebMessageType.requestObjectTagsUnderCharacter),
                new JProperty("payload", new JObject(
                    new JProperty("requestId", requestId)
                ))
            );
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(requestMessage));
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> sentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> expectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.requestObjectTagsUnderCharacterSuccess,
                payload = new JObject
                {
                    { "requestId", requestId },
                    { "tags", new JArray(platformTags)}
                }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
            yield return null;
        }
    }
}