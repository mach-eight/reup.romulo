using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwinTests.instantiators;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwinTests.mocks;

public class ReupPrefabTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;

    IObjectRegistry objectRegistry;

    EditMediator editMediator;
    EditModeManager editModeManager;


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        CreateComponents();
        yield return null;
    }

    private void CreateComponents()
    {
        sceneObjects = ReupSceneInstantiator.InstantiateScene();
        GameObject baseGlobalScriptGameObject = sceneObjects.baseGlobalScriptGameObject;
        objectRegistry = baseGlobalScriptGameObject.transform.Find("ObjectRegistry").GetComponent<IObjectRegistry>();

        GameObject editMediatorGameObject = baseGlobalScriptGameObject.transform.Find("EditMediator").gameObject;
        editMediator = editMediatorGameObject.GetComponent<EditMediator>();
        editModeManager = editMediatorGameObject.transform.Find("EditModeManager").GetComponent<EditModeManager>();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EditMediatorShouldFindTheRegistry()
    {
        Assert.AreEqual(objectRegistry, editMediator.registry);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EditMediatorShouldHaveAChangeMaterialController()
    {
        Assert.IsNotNull(editMediator.changeMaterialController);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighter_should_have_objectSensor()
    {
        Assert.IsNotNull(sceneObjects.selectableObjectHighlighter.objectSensor);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighter_should_have_objectHighlighter()
    {
        Assert.IsNotNull(sceneObjects.selectableObjectHighlighter.objectHighlighter);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighter_should_have_selectedObjectsManager()
    {
        Assert.IsNotNull(sceneObjects.selectableObjectHighlighter.selectedObjectsManager);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighterObjectSensor_should_haveSelectableObjectSelector()
    {
        ObjectSensor selectableObjectSensorHighligherObjectSensor = (ObjectSensor)sceneObjects.selectableObjectHighlighter.objectSensor;
        Assert.AreEqual(typeof(SelectableObjectSelector), selectableObjectSensorHighligherObjectSensor.objectSelector.GetType());
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectsHighlighter_should_beEnabled_onlyWhen_EditModeIsEnabled()
    {
        Assert.IsFalse(sceneObjects.selectableObjectHighlighter.enableHighlighting);

        editModeManager.editMode = true;
        yield return null;
        Assert.IsTrue(sceneObjects.selectableObjectHighlighter.enableHighlighting);
        editModeManager.editMode = false;
        yield return null;
        Assert.IsFalse(sceneObjects.selectableObjectHighlighter.enableHighlighting);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EditMediatorShouldHaveAOriginalSceneController()
    {
        Assert.IsNotNull(editMediator.originalSceneController);
        yield return null;
    }
    public IEnumerator SelectSelectableObject_ShouldHaveA_ISelectableObjectsHighlighter()
    {
        ISelectableObjectsHighlighter selectableObjectsHighlighter = sceneObjects.selectSelectableObject.selectableObjectsHighlighter;
        Assert.IsNotNull(selectableObjectsHighlighter);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectObjectManager_ShouldHaveA_IHighlightAnimator()
    {
        IHighlightAnimator highlightAnimator = sceneObjects.selectedObjectsManager.highlightAnimator;
        Assert.IsNotNull(highlightAnimator);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectObjectManager_ShouldHaveA_IObjectRegistry()
    {
        IObjectRegistry objectRegistry = sceneObjects.selectedObjectsManager.objectRegistry;
        Assert.IsNotNull(objectRegistry);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EditMediatorShouldHaveAViewModeController()
    {
        Assert.IsNotNull(editMediator.viewModeManager);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ViewModeControllerShouldHaveADHVCameraProperty()
    {
        ViewModeManager viewModeManager = sceneObjects.viewModeManager;
        Assert.IsNotNull(viewModeManager.dollhouseViewWrapper);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ViewModeControllerShouldHaveACharacterProperty()
    {
        ViewModeManager viewModeManager = sceneObjects.viewModeManager;
        Assert.IsNotNull(viewModeManager.character);
        yield return null;
    }

    [UnityTest]
    public IEnumerator DHVCameraMovementShouldHaveADHVCameraTransformHandler()
    {
        Assert.IsNotNull(sceneObjects.moveDhvCameraBehavior.dollhouseViewWrapperTransform);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EditMediatorCanImpedeCharacterRotation_when_transformHandleInteractionHappens()
    {
        Assert.IsTrue(sceneObjects.characterRotationManager.allowRotation);
        sceneObjects.editMediator.Notify(ReupEvent.transformHandleStartInteraction);
        Assert.IsFalse(sceneObjects.characterRotationManager.allowRotation);
        sceneObjects.editMediator.Notify(ReupEvent.transformHandleStopInteraction);
        Assert.IsTrue(sceneObjects.characterRotationManager.allowRotation);
        yield return null;
    }

}
