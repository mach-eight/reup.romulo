using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using Newtonsoft.Json;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwinTests.mocks;
using System.Linq;

namespace ReupVirtualTwinTests.generalTests
{
    public class SwitchViewModeTest
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        EditMediator editMediator;
        WebMessageSenderSpy webMessageSenderSpy;
        ViewModeManager viewModelManager;
        Transform character;
        GameObject dhvCamera;
        GameObject fpvCamera;
        List<GameObject> cameras;
        string activateDHVRequestId;
        string activateFPVRequestId;
        JObject activateDHVMessage;
        JObject activateFPVMessage;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            editMediator = sceneObjects.editMediator;
            webMessageSenderSpy = sceneObjects.webMessageSenderSpy;
            viewModelManager = sceneObjects.viewModeManager;
            character = sceneObjects.character;
            dhvCamera = sceneObjects.dhvCamera;
            fpvCamera = sceneObjects.fpvCamera;
            cameras = new List<GameObject> { fpvCamera, dhvCamera };
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        void DefineMessages()
        {
            activateDHVRequestId = "activateDHVRequestId";
            activateFPVRequestId = "activateFPVRequestId";
            activateDHVMessage = new JObject
            {
                { "type", WebMessageType.activateViewMode },
                { "payload", new JObject{
                    { "viewMode", ViewMode.dollhouse.ToString() },
                    { "requestId", activateDHVRequestId }
                }}
            };
            activateFPVMessage = new JObject
            {
                { "type", WebMessageType.activateViewMode },
                { "payload", new JObject{
                    { "viewMode", ViewMode.firstPerson.ToString() },
                    { "requestId", activateFPVRequestId }
                }}
            };
        }

        private void AssertOnlyOneCameraIsActive(GameObject expectedActiveCamera)
        {
            Assert.IsTrue(expectedActiveCamera.activeInHierarchy);
            List<GameObject> restOfCameras = cameras.Where(camera => camera != expectedActiveCamera).ToList();
            foreach (GameObject camera in restOfCameras)
            {
                Assert.IsFalse(camera.activeInHierarchy);
            }
        }

        [Test]
        public void ShouldHaveFPVActivatedByDefault()
        {
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.firstPerson);
        }

        [Test]
        public void ShouldHaveDHVCameraWrapperDeactivatedByDefault()
        {
            Assert.IsFalse(dhvCamera.activeInHierarchy);
            Assert.IsFalse(sceneObjects.dollhouseViewWrapper.gameObject.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator ShouldActivateDHV_when_receiveMessage()
        {
            DefineMessages();
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateDHVMessage));
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.dollhouse);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldActivateDHVCamera_when_changeViewModeToDHV()
        {
            DefineMessages();
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateDHVMessage));
            AssertOnlyOneCameraIsActive(dhvCamera);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldChangeBackViewModeToFPV()
        {
            DefineMessages();
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateDHVMessage));
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.dollhouse);
            yield return null;
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateFPVMessage));
            Assert.AreEqual(viewModelManager.viewMode, ViewMode.firstPerson);
        }

        [UnityTest]
        public IEnumerator ShouldActivateFPVCamera_when_changeViewModeToFPV()
        {
            DefineMessages();
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateDHVMessage));
            yield return null;
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateFPVMessage));
            AssertOnlyOneCameraIsActive(fpvCamera);
        }

        [UnityTest]
        public IEnumerator CharacterShouldBeDeactivated_when_inDHV()
        {
            DefineMessages();
            Assert.IsTrue(character.gameObject.activeSelf);
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateDHVMessage));
            Assert.IsFalse(character.gameObject.activeSelf);
            yield return null;
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateFPVMessage));
            Assert.IsTrue(character.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ShouldSendDHVSuccessMessage()
        {
            DefineMessages();
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateDHVMessage));
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> sentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> expectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.activateViewModeSuccess,
                payload = new JObject
                {
                    { "viewMode", ViewMode.dollhouse.ToString() },
                    { "requestId", activateDHVRequestId }
                }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldSendFPVSuccessMessage()
        {
            DefineMessages();
            editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(activateFPVMessage));
            Assert.AreEqual(1, webMessageSenderSpy.sentMessages.Count);
            WebMessage<JObject> sentMessage = (WebMessage<JObject>)webMessageSenderSpy.sentMessages[0];
            WebMessage<JObject> expectedMessage = new WebMessage<JObject>
            {
                type = WebMessageType.activateViewModeSuccess,
                payload = new JObject
                {
                    { "viewMode", ViewMode.firstPerson.ToString() },
                    { "requestId", activateFPVRequestId }
                }
            };
            Assert.IsTrue(JObject.DeepEquals(expectedMessage.payload, sentMessage.payload));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldSendErrorMessage_when_ReceiveActivateViewModeMessageWithoutRequestId()
        {
            JObject message = new JObject
            {
                { "type", WebMessageType.activateViewMode },
                { "payload", new JObject()
                    {
                        { "viewMode", ViewMode.dollhouse.ToString() }
                    }
                }
            };
            editMediator.ReceiveWebMessage(message.ToString());
            yield return null;
            WebMessage<string[]> sentMessage = (WebMessage<string[]>)webMessageSenderSpy.sentMessages[0];
            Assert.AreEqual(WebMessageType.error, sentMessage.type);
        }

        [UnityTest]
        public IEnumerator ShouldSendErrorMessage_when_ReceiveActivateViewModeMessageWithoutViewMode()
        {
            JObject message = new JObject
            {
                { "type", WebMessageType.activateViewMode },
                { "payload", new JObject()
                    {
                        { "requestId", "UUID" }
                    }
                }
            };
            editMediator.ReceiveWebMessage(message.ToString());
            yield return null;
            WebMessage<string[]> sentMessage = (WebMessage<string[]>)webMessageSenderSpy.sentMessages[0];
            Assert.AreEqual(WebMessageType.error, sentMessage.type);
        }

        [UnityTest]
        public IEnumerator ShouldSendErrorMessage_When_ReceiveActivateViewModeMessageWithInvalidViewMode()
        {
            JObject message = new JObject
            {
                { "type", WebMessageType.activateViewMode },
                { "payload", new JObject()
                    {
                        { "requestId", "UUID" },
                        { "viewMode", "invalid-view-mode" }
                    }
                }
            };
            editMediator.ReceiveWebMessage(message.ToString());
            yield return null;
            WebMessage<string[]> sentMessage = (WebMessage<string[]>)webMessageSenderSpy.sentMessages[0];
            Assert.AreEqual(WebMessageType.error, sentMessage.type);
        }
    }

}