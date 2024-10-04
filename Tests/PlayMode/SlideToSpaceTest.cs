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

namespace ReupVirtualTwinTests.generalTests
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
                    { "message", $"Space jump point with id '{nonExistentSpaceJumpPointId}' not found" },
                    { "spaceId", nonExistentSpaceJumpPointId },
                }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
        }

        [UnityTest]
        public IEnumerator ShouldSendFailMessage_when_requestedJumpPointHasNoGroundBelow()
        {
            DefineConstants();
            spaceSelectors = new List<GameObject>(){
                SpaceSelectorFabric.Create(new SpaceSelectorFabric.SpaceSelectorConfig
                {
                    id = spaceJumpPointId,
                    position = new Vector3(100,100,100),
                })
            };
            spaceSelectors[0].transform.position = new Vector3(100, 100, 100);
            yield return editMediator.ReceiveWebMessage(serializedMessage);
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> sentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> expectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceFailure,
                payload = new JObject
                {
                    { "requestId", requestId },
                    { "message", $"Space jump point with id '{spaceJumpPointId}' has no ground below to jump to" },
                    { "spaceId", spaceJumpPointId },
                }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldSendSlideToSpaceFailMessage_when_anotherSlideToSpaceRequestInterruptTheCurrentOne()
        {
            spaceSelectors = SpaceSelectorFabric.CreateBulk(2);
            spaceSelectors[0].transform.position = new Vector3(1, 1, 1);
            spaceSelectors[1].transform.position = new Vector3(-0.1f, -0.1f, -0.1f);
            string id1 = "space-jump-point-1";
            string id2 = "space-jump-point-2";
            spaceSelectors[0].GetComponent<SpaceJumpPoint>().id = id1;
            spaceSelectors[1].GetComponent<SpaceJumpPoint>().id = id2;
            spacesRecord.UpdateSpaces();
            yield return null;
            string requestId1 = "request-id-1";
            string requestId2 = "request-id-2";
            JObject goToSpace1Message = new JObject
            {
                { "type", WebMessageType.slideToSpace },
                { "payload", new JObject
                    {
                        { "spaceId", id1 },
                        { "requestId", requestId1 },
                    }
                }
            };
            JObject goToSpace2Message = new JObject
            {
                { "type", WebMessageType.slideToSpace },
                { "payload", new JObject
                    {
                        { "spaceId", id2 },
                        { "requestId", requestId2 },
                    }
                }
            };
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(goToSpace1Message));
            yield return null;
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(goToSpace2Message));
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> failSentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> failExpectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceFailure,
                payload = new JObject
                {
                    { "message", "Slide to space point was interrupted" },
                    { "spaceId", id1 },
                    { "requestId", requestId1 }
                }
            };
            Assert.IsTrue(JObject.DeepEquals(failExpectedMessage.payload, failSentMessage.payload));
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(2, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> successSentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[1];
            WebMessage<JObject> successExpectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceSuccess,
                payload = new JObject
            {
                { "spaceId", id2 },
                { "requestId", requestId2 }
            }
            };
            Assert.IsTrue(JObject.DeepEquals(successExpectedMessage.payload, successSentMessage.payload));
        }

    }
}