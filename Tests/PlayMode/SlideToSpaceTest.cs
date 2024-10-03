using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEditor;
using ReupVirtualTwinTests.mocks;
using ReupVirtualTwin.managerInterfaces;
using Newtonsoft.Json.Linq;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using Newtonsoft.Json;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwinTests.managers
{
    public class SlideToSpaceTest : MonoBehaviour
    {
        GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Platform2.prefab");
        ReupSceneInstantiator.SceneObjects sceneObjects;
        SpacesRecord spacesRecord;
        List<GameObject> spaceSelectors;
        WebMessageSenderSpy webMessageSenderSpy;
        Transform character;
        float lowPlatformHeight = -3;
        EditMediator editMediator;
        string spaceJumpPointId;
        string requestId;
        float characterYPosition;
        JObject goToSpacePointMessage;
        string serializedMessage;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab, SetLowPlatform);
            spacesRecord = sceneObjects.spacesRecord;
            character = sceneObjects.character;
            editMediator = sceneObjects.editMediator;
            webMessageSenderSpy = sceneObjects.webMessageSenderSpy;
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            SpaceSelectorFabric.DestroySpaceSelectors(spaceSelectors);
            spaceSelectors?.Clear();
            yield return null;
        }

        void DefineConstants()
        {
            spaceJumpPointId = "space-jump-point-1";
            requestId = "space-id-1";
            characterYPosition = character.position.y;
            goToSpacePointMessage = new JObject
            {
                { "type", WebMessageType.slideToSpace },
                { "payload", new JObject
                    {
                        { "spaceId", spaceJumpPointId },
                        { "requestId", requestId },
                    }
                }
            };
            serializedMessage = JsonConvert.SerializeObject(goToSpacePointMessage);
        }

        void SetLowPlatform(GameObject platform)
        {
            platform.transform.position = new Vector3(0, lowPlatformHeight, 0);
        }

        [UnityTest]
        public IEnumerator SpacesListShouldBeAnEmptyList_if_noSpacesExist()
        {
            Assert.AreEqual(0, spacesRecord.jumpPoints.Count);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SpacesListShouldContainAllSpaceJumpPointsInTheScene()
        {
            DefineConstants();
            spaceSelectors = SpaceSelectorFabric.CreateBulk(5);
            yield return null;
            Assert.AreEqual(5, spacesRecord.jumpPoints.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(spaceSelectors[i].GetComponent<SpaceJumpPoint>(), spacesRecord.jumpPoints[i]);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterShouldMoveToSpaceJumpPoint()
        {
            DefineConstants();
            Vector3 spaceJumpPointPosition = new Vector3(0.2f, 2, 0.3f);
            spaceSelectors = new List<GameObject>(){
                SpaceSelectorFabric.Create(new SpaceSelectorFabric.SpaceSelectorConfig
                {
                    id = spaceJumpPointId,
                    position = spaceJumpPointPosition,
                })
            };
            yield return null;
            yield return editMediator.ReceiveWebMessage(serializedMessage);
            yield return new WaitForSeconds(0.3f);
            Vector3 desiredPositionAfterJump = new Vector3(spaceJumpPointPosition.x, characterYPosition, spaceJumpPointPosition.z);
            AssertUtils.AssertVectorsAreClose(desiredPositionAfterJump, character.position, 0.02);
        }

        [UnityTest]
        public IEnumerator ShouldSendSlideToSpaceSuccessMessage_when_jumpPointIsReachedByCharacter()
        {
            DefineConstants();
            Vector3 spaceJumpPointPosition = new Vector3(0.2f, 2, 0.3f);
            spaceSelectors = new List<GameObject>(){
                SpaceSelectorFabric.Create(new SpaceSelectorFabric.SpaceSelectorConfig
                {
                    id = spaceJumpPointId,
                    position = spaceJumpPointPosition,
                })
            };
            yield return null;
            yield return editMediator.ReceiveWebMessage(serializedMessage);
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> sentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> expectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceSuccess,
                payload = new JObject
            {
                { "spaceId", spaceJumpPointId },
                { "requestId", requestId }
            }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
        }

        [UnityTest]
        public IEnumerator ShouldSendFailMessage_when_requestedToGoToNonExistentSpaceJumpPoint()
        {
            DefineConstants();
            string nonExistentSpaceJumpPointId = "non-existent-space-jump-point";
            JObject message = new JObject
            {
                { "type", WebMessageType.slideToSpace },
                { "payload", new JObject
                    {
                        { "spaceId", nonExistentSpaceJumpPointId },
                        { "requestId", requestId },
                    }
                }
            };
            yield return editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(message));
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> sentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> expectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceFailure,
                payload = new JObject
                {
                    { "requestId", requestId },
                    { "message", $"Space jump point with id {nonExistentSpaceJumpPointId} not found" },
                }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
        }

    }
}