using System.Collections;
using UnityEngine;
using ReupVirtualTwin.managers;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;

public class EditModeManagerTest : MonoBehaviour
{
    GameObject containerGameObject;
    EditModeManager editModeManager;
    MockMediator mockMediator;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject();
        editModeManager = containerGameObject.AddComponent<EditModeManager>();
        mockMediator = new MockMediator();
        editModeManager.mediator = mockMediator;
    }

    [UnityTest]
    public IEnumerator EditModeManagerShouldNotifyMediatorWhenSettingEditMode()
    {
        editModeManager.editMode = true;
        Assert.AreEqual(true, mockMediator.editMode);
        yield return null;
        editModeManager.editMode = false;
        Assert.AreEqual(false, mockMediator.editMode);
        yield return null;
    }

    private class MockMediator : IMediator
    {
        public bool editMode;
        public void Notify(ReupEvent eventName)
        {
            throw new System.NotImplementedException();
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            if (eventName == ReupEvent.setEditMode)
            {
                editMode = (bool)(object)payload;
            }
        }
    }

}
