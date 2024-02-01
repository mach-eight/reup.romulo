using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.managers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helperInterfaces;
using System.Collections.Generic;

public class EditionMediatorTest : MonoBehaviour
{
    GameObject containerGameObject;
    EditionMediator editionMediator;
    MockEditModeManager mockEditModeManager;
    MockSelectedObjectsManager mockSelectedObjectsManager;
    MockWebMessageSender mockWebMessageSender;
    MockTransformObjectsManager mockTransformObjectsManager;
    MockInsertObjectsManager mockInsertObjectsManager;
    MockObjectMapper mockObjectMapper;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject();
        editionMediator = containerGameObject.AddComponent<EditionMediator>();
        mockEditModeManager = new MockEditModeManager();
        mockSelectedObjectsManager = new MockSelectedObjectsManager();
        editionMediator.editModeManager = mockEditModeManager;
        editionMediator.selectedObjectsManager = mockSelectedObjectsManager;
        mockWebMessageSender = new MockWebMessageSender();
        editionMediator.webMessageSender = mockWebMessageSender;
        mockTransformObjectsManager = new MockTransformObjectsManager();
        editionMediator.transformObjectsManager = mockTransformObjectsManager;
        mockInsertObjectsManager = new MockInsertObjectsManager();
        editionMediator.insertObjectsManager = mockInsertObjectsManager;
        mockObjectMapper = new MockObjectMapper();
        editionMediator.objectMapper = mockObjectMapper;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToTrue()
    {
        editionMediator.Notify(Events.setEditMode, true);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(true, sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToFalse()
    {
        editionMediator.Notify(Events.setEditMode, false);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(false, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSetEditModeWhenReceiveRequest()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.setEditMode, "true");
        editionMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, true);
        yield return null;
        message = dummyJsonCreator.createWebMessage(WebMessageType.setEditMode, "false");
        editionMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, false);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAllowAndDisallowObjectSelection()
    {
        editionMediator.Notify(Events.setEditMode, true);
        Assert.AreEqual(mockSelectedObjectsManager.allowSelection, true);
        yield return null;
        editionMediator.Notify(Events.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.allowSelection, false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearSelectionWhenEditModeIsSetToFalse()
    {
        editionMediator.Notify(Events.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.selectionCleared, true);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToActivatePositionTransformModeWithNoSelectedObject()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.activatePositionTransform);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual($"Can't activate {TransformMode.PositionMode} Transform mode because no object is selected", sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToActivateRotationTransformModeWithNoSelectedObject()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.activateRotationTransform);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual($"Can't activate {TransformMode.RotationMode} Transform mode because no object is selected", sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToDeactivateTransformModeButNoTransforModeIsActive()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.deactivateTransformMode);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual("Can't deactivate transform mode if no transform mode is currently active", sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldOrderInsertObjectManagerToInsertObject()
    {
        string mockUrl = "the mock url";
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, $"\"{mockUrl}\"");
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsTrue(mockInsertObjectsManager.calledToInsertObject);
        Assert.AreEqual(mockUrl, mockInsertObjectsManager.objectLoadString);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectUpdatedProcess()
    {
        float processStatus = 0.25f;
        editionMediator.Notify(Events.insertedObjectStatusUpdate, processStatus);
        WebMessage<float> sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
        processStatus = 0.6f;
        editionMediator.Notify(Events.insertedObjectStatusUpdate, processStatus);
        sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectSuccess()
    {
        GameObject insertedObject = new GameObject("insertedObject");
        editionMediator.Notify(Events.insertedObjectLoaded, insertedObject);
        WebMessage<ObjectDTO> sentMessage = (WebMessage<ObjectDTO>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectSuccess, sentMessage.type);
        Assert.AreEqual(mockObjectMapper.objectDTOs[0], sentMessage.payload);
        yield return null;
    }

    private class MockEditModeManager : IEditModeManager
    {
        private bool _editMode;
        public bool editMode { get => _editMode; set => _editMode = value; }
    }
    private class MockSelectedObjectsManager : ISelectedObjectsManager
    {
        public MockSelectedObjectsManager()
        {
            wrapperDTO = new ObjectWrapperDTO()
            {
                wrapper = null,
                wrappedObjects = null,
            };
        }
        private bool _allowSelection = false;
        public bool allowSelection { get => _allowSelection; set => _allowSelection = value; }


        public bool selectionCleared = false;
        private ObjectWrapperDTO _wraperDTO;
        public ObjectWrapperDTO wrapperDTO { get => _wraperDTO; set => _wraperDTO = value; }

        public GameObject AddObjectToSelection(GameObject selectedObject)
        {
            throw new System.NotImplementedException();
        }

        public void ClearSelection()
        {
            selectionCleared = true;
        }

        public GameObject RemoveObjectFromSelection(GameObject selectedObject)
        {
            throw new System.NotImplementedException();
        }
    }

    private class MockWebMessageSender : IWebMessagesSender
    {
        public object sentMessage;
        public void SendWebMessage<T>(WebMessage<T> webWessage)
        {
            sentMessage = webWessage;
        }
    }
    private class MockTransformObjectsManager : ITransformObjectsManager
    {
        public bool _active = false;
        public bool active => _active;

        public ObjectWrapperDTO wrapper { set => throw new System.NotImplementedException(); }

        public void ActivateTransformMode(GameObject wrapper, TransformMode mode)
        {
            _active = true;
        }

        public void ActivateTransformMode(ObjectWrapperDTO wrapper, TransformMode mode)
        {
            throw new System.NotImplementedException();
        }

        public void DeactivateTransformMode()
        {
            _active = false;
        }
    }

    private class MockInsertObjectsManager : IInsertObjectsManager
    {
        public bool calledToInsertObject = false;
        public string objectLoadString = null;
        public void InsertObjectFromUrl(string url)
        {
            calledToInsertObject = true;
            objectLoadString = url;
        }
    }
    private class MockObjectMapper : IObjectMapper
    {
        public ObjectDTO[] objectDTOs = new ObjectDTO[2]
        {
            new ObjectDTO
            {
                id = "id0",
                tags = new string[2]{"tag0", "tag1"},
            },
            new ObjectDTO
            {
                id = "id1",
                tags = new string[2]{"tag2", "tag3"},
            },
        };
        public ObjectDTO[] MapObjectsToDTO(List<GameObject> objs)
        {
            return objectDTOs;
        }

        public ObjectDTO MapObjectToDTO(GameObject obj)
        {
            return objectDTOs[0];
        }
    }
    private static class dummyJsonCreator
    {
        public static string createWebMessage(string type)
        {
            return $"{{\"type\":\"{type}\"}}";
        }
        public static string createWebMessage(string type, string payload)
        {
            return $"{{\"type\":\"{type}\",\"payload\":{payload}}}";
        }
    }

}
