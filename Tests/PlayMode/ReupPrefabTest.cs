using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.managers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.helpers;

public class ReupPrefabTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;
    GameObject character;

    IObjectRegistry objectRegistry;

    EditMediator editMediator;
    SensedObjectHighlighter selectableObjectHighlighter;
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

        character = sceneObjects.character;
        selectableObjectHighlighter = character.transform
            .Find("Behaviours")
            .Find("HoverOverSelectablesObjects")
            .gameObject.GetComponent<SensedObjectHighlighter>();
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
        Assert.IsNotNull(selectableObjectHighlighter.objectSensor);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighter_should_have_objectHighlighter()
    {
        Assert.IsNotNull(selectableObjectHighlighter.objectHighlighter);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighter_should_have_selectedObjectsManager()
    {
        Assert.IsNotNull(selectableObjectHighlighter.selectedObjectsManager);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectHighlighterObjectSensor_should_haveSelectableObjectSelector()
    {
        ObjectSensor selectableObjectSensorHighligherObjectSensor = (ObjectSensor) selectableObjectHighlighter.objectSensor;
        Assert.AreEqual(typeof(SelectableObjectSelector), selectableObjectSensorHighligherObjectSensor.objectSelector.GetType());
        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectableObjectsHighlighter_should_beEnabled_onlyWhen_EditModeIsEnabled()
    {
        Assert.IsFalse(selectableObjectHighlighter.enableHighlighting);

        editModeManager.editMode = true;
        yield return null;
        Assert.IsTrue(selectableObjectHighlighter.enableHighlighting);
        editModeManager.editMode = false;
        yield return null;
        Assert.IsFalse(selectableObjectHighlighter.enableHighlighting);
        yield return null;
    }

}
