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
using ReupVirtualTwin.controllerInterfaces;

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
        mockInsertObjectsManager = new MockInsertObjectsManager(editionMediator);
        editionMediator.insertObjectsController = mockInsertObjectsManager;
        mockObjectMapper = new MockObjectMapper();
        editionMediator.objectMapper = mockObjectMapper;
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
            if (wrapperDTO.wrapper == null)
            {
                wrapperDTO = new ObjectWrapperDTO()
                {
                    wrapper = new GameObject("wrapper"),
                    wrappedObjects = new List<GameObject>(),
                };
            }
            wrapperDTO.wrappedObjects.Add(selectedObject);
            return wrapperDTO.wrapper;
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

    private class MockInsertObjectsManager : IInsertObjectsController
    {
        public GameObject injectedObject = null;
        public bool calledToInsertObject = false;
        public string objectLoadString = null;
        public string requestedObjectId;
        private IMediator editionMediator;
        public MockInsertObjectsManager(IMediator mediator)
        {
            calledToInsertObject = false;
            objectLoadString = null;
            editionMediator = mediator;
        }

        public void InsertObject(InsertObjectMessagePayload insertObjectMessagePayload)
        {
            injectedObject = new GameObject("injected test object");
            calledToInsertObject = true;
            objectLoadString = insertObjectMessagePayload.objectUrl;
            InsertedObjectPayload insertedObjectPayload = new()
            {
                loadedObject = injectedObject,
                selectObjectAfterInsertion = insertObjectMessagePayload.selectObjectAfterInsertion,
                deselectPreviousSelection = insertObjectMessagePayload.deselectPreviousSelection,
            };
            editionMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
            requestedObjectId = insertObjectMessagePayload.objectId;
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

        public ObjectDTO MapObjectTree(GameObject obj)
        {
            throw new System.NotImplementedException();
        }
    }
    private static class dummyJsonCreator
    {
        public static string createWebMessage(string type)
        {
            return $"{{\"type\":\"{type}\"}}";
        }
        public static string createWebMessage(string type, object payload)
        {
            if (payload is int || payload is float)
            {
                return $"{{\"type\":\"{type}\",\"payload\":{payload}}}";
            }
            string processedPayload;
            if (payload is string)
            {
                processedPayload = payload.ToString();
            }
            else if (payload is bool)
            {
                processedPayload = payload.ToString().ToLower();
            }
            else
            {
                processedPayload = JsonUtility.ToJson(payload);
            }
            processedPayload = ScapeSpecialChars(processedPayload);
            return $"{{\"type\":\"{type}\",\"payload\":\"{processedPayload}\"}}";

        }
        static string ScapeSpecialChars(string str)
        {
            return str.Replace("\"", "\\\"");
        }
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToTrue()
    {
        editionMediator.Notify(ReupEvent.setEditMode, true);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(true, sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToFalse()
    {
        editionMediator.Notify(ReupEvent.setEditMode, false);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(false, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSetEditModeWhenReceiveRequest()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.setEditMode, true);
        editionMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, true);
        yield return null;
        message = dummyJsonCreator.createWebMessage(WebMessageType.setEditMode, false);
        editionMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, false);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAllowAndDisallowObjectSelection()
    {
        editionMediator.Notify(ReupEvent.setEditMode, true);
        Assert.AreEqual(mockSelectedObjectsManager.allowSelection, true);
        yield return null;
        editionMediator.Notify(ReupEvent.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.allowSelection, false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearSelectionWhenEditModeIsSetToFalse()
    {
        editionMediator.Notify(ReupEvent.setEditMode, false);
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
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-object-id",
            selectObjectAfterInsertion = true,
            deselectPreviousSelection = true,
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsTrue(mockInsertObjectsManager.calledToInsertObject);
        Assert.AreEqual(payload.objectUrl, mockInsertObjectsManager.objectLoadString);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectUpdatedProcess()
    {
        float processStatus = 0.25f;
        editionMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        WebMessage<float> sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
        processStatus = 0.6f;
        editionMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectSuccess()
    {
        InsertedObjectPayload insertedObjectPayload = new()
        {
            loadedObject = new GameObject("insertedObject"),
        };
        editionMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        WebMessage<ObjectDTO> sentMessage = (WebMessage<ObjectDTO>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectSuccess, sentMessage.type);
        Assert.AreEqual(mockObjectMapper.objectDTOs[0], sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldNotSelectInsertedObjectByDefault()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-3d-model-url",
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsNull(mockSelectedObjectsManager.wrapperDTO.wrappedObjects);
        Assert.IsNull(mockSelectedObjectsManager.wrapperDTO.wrappedObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceiveInsertObjectActionWithoutObjectUrl()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectId = "test-3d-model-url",
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editionMediator.noInsertObjectUrlErrorMessage, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceiveInsertObjectActionWithoutObjectId()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editionMediator.noInsertObjectIdErrorMessage, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSelectJustInsertedObject()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-object-id",
            selectObjectAfterInsertion = true,
            deselectPreviousSelection = true,
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsTrue(mockSelectedObjectsManager.wrapperDTO.wrappedObjects.Contains(mockInsertObjectsManager.injectedObject));
        Assert.AreEqual(1, mockSelectedObjectsManager.wrapperDTO.wrappedObjects.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldRequestAddPredefinedIdToParentObject()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-object-id",
            selectObjectAfterInsertion = true,
            deselectPreviousSelection = true,
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.AreEqual(payload.objectId, mockInsertObjectsManager.requestedObjectId);
    }

}
