using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;

using ReupVirtualTwin.managers;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine.InputSystem;

public class CollisionDetectorTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;
    Transform character;

    ICharacterPositionManager posManager;
    GameObject cubePrefab;
    GameObject widePlatform;
    GameObject wall;
    Keyboard keyboard;
    InputTestFixture input;

    [SetUp]
    public void SetUp()
    {
        cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Cube.prefab");
        sceneObjects = ReupSceneInstantiator.InstantiateScene();
        character = sceneObjects.character;
        posManager = sceneObjects.characterPositionManager;
        wall = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        widePlatform = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        keyboard = InputSystem.AddDevice<Keyboard>();
        input = sceneObjects.input;
        SetPlatform();
        SetWallAt2MetersInZAxis();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        GameObject.DestroyImmediate(widePlatform);
        GameObject.DestroyImmediate(wall);
        ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CharacterShouldNotCrossWall()
    {
        character.transform.position = new Vector3(0, 1.5f, 0);
        yield return new WaitForSeconds(0.2f);

        posManager.WalkToTarget(new Vector3(0, 0, 5));

        yield return new WaitForSeconds(2f);

        Assert.LessOrEqual(character.transform.position.z, 2);
    }
    [UnityTest]
    public IEnumerator CharacterShouldNotCrossWallWhileMovingWithKeyboard()
    {
        input.Press(keyboard.wKey);
        yield return new WaitForSeconds(1);
        input.Release(keyboard.wKey);
        Assert.LessOrEqual(character.transform.position.z, 2);
    }
    private void SetPlatform()
    {
        widePlatform.transform.localScale = new Vector3(10, 0.1f, 10);
        widePlatform.transform.position = new Vector3(0, -0.05f, 0);
    }
    private void SetWallAt2MetersInZAxis()
    {
        wall.transform.localScale = new Vector3(10, 10, 0.1f);
        wall.transform.position = new Vector3(0, 0, 2.05f);
    }
}
