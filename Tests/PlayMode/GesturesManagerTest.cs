using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ReupVirtualTwin.managers;
using ReupVirtualTwinTests.utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class GesturesManagerTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;
    GesturesManager gesturesManager;
    InputTestFixture input;
    Touchscreen touch;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        sceneObjects = ReupSceneInstantiator.InstantiateScene();
        gesturesManager = sceneObjects.gesturesManager;
        input = sceneObjects.input;
        touch = InputSystem.AddDevice<Touchscreen>();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        yield return null;
    }
    [UnityTest]
    public IEnumerator NoTouch_ShouldNotTriggerGesture()
    {
        yield return null;
        Assert.IsFalse(gesturesManager.gestureInProgress);
    }

    [UnityTest]
    public IEnumerator OneTouch_ShouldNotTriggerGesture()
    {
        input.BeginTouch(0, new Vector2(100, 100), true, touch);
        yield return null;
        Assert.IsFalse(gesturesManager.gestureInProgress);
    }

    [UnityTest]
    public IEnumerator TwoTouches_ShouldTriggerGesture()
    {
        input.BeginTouch(0, new Vector2(100, 100), true, touch);
        input.BeginTouch(1, new Vector2(100, 200), true, touch);
        yield return null;
        Assert.IsTrue(gesturesManager.gestureInProgress);
    }

    [UnityTest]
    public IEnumerator TwoTouchesMove_ShouldMaintainGesture()
    {
        input.BeginTouch(0, new Vector2(100, 100), true, touch);
        input.BeginTouch(1, new Vector2(200, 200), true, touch);
        yield return null;
        Assert.IsTrue(gesturesManager.gestureInProgress);

        input.MoveTouch(0, new Vector2(150, 150));
        input.MoveTouch(1, new Vector2(250, 250));
        yield return null;
        Assert.IsTrue(gesturesManager.gestureInProgress);
    }

    [UnityTest]
    public IEnumerator EndTouches_ShouldEndGesture()
    {
        Assert.IsFalse(gesturesManager.gestureInProgress);

        input.BeginTouch(0, new Vector2(100, 100), true, touch);
        input.BeginTouch(1, new Vector2(200, 200), true, touch);
        yield return null;
        Assert.IsTrue(gesturesManager.gestureInProgress);

        input.EndTouch(0, new Vector2(150, 150), Vector2.zero, true, touch);
        input.EndTouch(1, new Vector2(250, 250), Vector2.zero, true, touch);
        yield return null;
        Assert.IsFalse(gesturesManager.gestureInProgress);
    }
}
