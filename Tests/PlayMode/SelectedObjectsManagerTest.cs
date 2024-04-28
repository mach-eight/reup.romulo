using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.managers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.models;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.dataModels;

public class SelectedObjectsManagerTest : MonoBehaviour
{
    GameObject containerGameObject;
    GameObject testGameObject0;
    GameObject testGameObject1;
    SelectedObjectsManager selectedObjectsManager;
    MockMediator mockMediator;
    MockHighlighter mockHighlighter;
    MockObjectWrapper mockObjectWrapper;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject();
        testGameObject0 = new GameObject("testGameObject0");
        testGameObject0.AddComponent<UniqueId>();
        testGameObject1 = new GameObject("testGameObject1");
        testGameObject1.AddComponent<UniqueId>();
        selectedObjectsManager = containerGameObject.AddComponent<SelectedObjectsManager>();
        mockMediator = new MockMediator();
        mockHighlighter = new MockHighlighter();
        mockObjectWrapper = new MockObjectWrapper();
        selectedObjectsManager.mediator = mockMediator;
        selectedObjectsManager.highlighter = mockHighlighter;
        selectedObjectsManager.objectWrapper = mockObjectWrapper;
    }

    [UnityTest]
    public IEnumerator ShouldNotifyMediatorOfSelectObjects()
    {
        selectedObjectsManager.allowSelection = true;
        selectedObjectsManager.AddObjectToSelection(testGameObject0);
        Assert.AreEqual(new List<GameObject>() { testGameObject0}, mockMediator.selectedObjects);
        selectedObjectsManager.AddObjectToSelection(testGameObject1);
        Assert.AreEqual(new List<GameObject>() { testGameObject0, testGameObject1}, mockMediator.selectedObjects);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldNotSelectAnyObjectIfAllowSelectionNotSet()
    {
        selectedObjectsManager.allowSelection = false;
        selectedObjectsManager.AddObjectToSelection(testGameObject0);
        selectedObjectsManager.AddObjectToSelection(testGameObject1);
        Assert.AreEqual(new List<GameObject>() {}, mockMediator.selectedObjects);
        yield return null;
        selectedObjectsManager.allowSelection = true;
        selectedObjectsManager.AddObjectToSelection(testGameObject0);
        selectedObjectsManager.AddObjectToSelection(testGameObject1);
        Assert.AreEqual(new List<GameObject>() { testGameObject0, testGameObject1}, mockMediator.selectedObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearAllSelectedObjects()
    {

        selectedObjectsManager.allowSelection = true;
        selectedObjectsManager.AddObjectToSelection(testGameObject0);
        selectedObjectsManager.AddObjectToSelection(testGameObject1);
        Assert.AreEqual(new List<GameObject>() { testGameObject0, testGameObject1}, mockMediator.selectedObjects);
        yield return null;
        selectedObjectsManager.ClearSelection();
        Assert.AreEqual(new List<GameObject>() {}, mockMediator.selectedObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearAllSelectedObjectsWhenAllowSelectionIsUnset()
    {

        selectedObjectsManager.allowSelection = true;
        selectedObjectsManager.AddObjectToSelection(testGameObject0);
        selectedObjectsManager.AddObjectToSelection(testGameObject1);
        Assert.AreEqual(new List<GameObject>() { testGameObject0, testGameObject1}, mockMediator.selectedObjects);
        yield return null;
        selectedObjectsManager.allowSelection = false;
        Assert.AreEqual(new List<GameObject>() {}, mockMediator.selectedObjects);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldNotNotifyMediatorWhenClearingSelectionIfNoObjectIsSelected()
    {
        Assert.AreEqual(0, mockMediator.selectedObjects.Count);
        Assert.AreEqual(false, mockMediator.selectedObjectModified);
        yield return null;
        selectedObjectsManager.ClearSelection();
        Assert.AreEqual(false, mockMediator.selectedObjectModified);
    }
    [UnityTest]
    public IEnumerator ShouldDestroyWrapperObjectAfterClearingSelection()
    {
        selectedObjectsManager.allowSelection = true;
        selectedObjectsManager.AddObjectToSelection(testGameObject0);
        Assert.NotNull(selectedObjectsManager.wrapperDTO);
        yield return null;
        GameObject selection = selectedObjectsManager.wrapperDTO.wrapper;
        selectedObjectsManager.ClearSelection();
        yield return null;
        Assert.IsTrue(selection == null);
        yield return null;
    }

    private class MockMediator : IMediator
    {
        public List<GameObject> selectedObjects = new List<GameObject>() { };
        public bool selectedObjectModified = false;
        public void Notify(ReupEvent eventName)
        {
            throw new System.NotImplementedException();
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            if (eventName == ReupEvent.setSelectedObjects)
            {
                selectedObjects = ((ObjectWrapperDTO)(object)payload).wrappedObjects;
                selectedObjectModified = true;
            }
        }
    }

    private class MockHighlighter : IObjectHighlighter
    {
        public void HighlightObject(GameObject obj)
        {
        }

        public void UnhighlightObject(GameObject obj)
        {
        }
    }

    private class MockObjectWrapper : IObjectWrapper
    {
        private List<GameObject> selectedObjects = new List<GameObject>();
        public List<GameObject> wrappedObjects => selectedObjects;

        public GameObject wrapper => throw new System.NotImplementedException();

        private GameObject _wrapper = new GameObject();

        public void DeWrapAll()
        {
            selectedObjects.Clear();
        }

        public GameObject UnwrapObject(GameObject obj)
        {
            selectedObjects.Remove(obj);
            return _wrapper;
        }

        public GameObject WrapObject(GameObject obj)
        {
            selectedObjects.Add(obj);
            return _wrapper;
        }

        public GameObject WrapObjects(GameObject[] objs)
        {
            foreach(GameObject obj in objs)
            {
                selectedObjects.Add(obj);
            }
            return _wrapper;
        }
    }

}
