using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;

using ReupVirtualTwin.managers;

public class CollisionDetectorTest : MonoBehaviour
{
    ReupPrefabInstantiator.SceneObjects sceneObjects;
    GameObject character;

    CharacterPositionManager posManager;
    GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Cube.prefab");
    GameObject widePlatform;
    GameObject wall;

    [SetUp]
    public void SetUp()
    {
        sceneObjects = ReupPrefabInstantiator.InstantiateScene();
        character = sceneObjects.character;
        posManager = character.GetComponent<CharacterPositionManager>();
        widePlatform = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        SetPlatform();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Destroy(widePlatform);
        Destroy(wall);
        ReupPrefabInstantiator.DestroySceneObjects(sceneObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator CharacterShouldNotCrossWall()
    {
        wall = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        SetWallAt2MetersInZAxis();

        character.transform.position = new Vector3(0, 1.5f, 0);
        yield return new WaitForSeconds(0.2f);

        posManager.WalkToTarget(new Vector3(0, 0, 5));

        yield return new WaitForSeconds(2f);

        Assert.LessOrEqual(character.transform.position.z, 2);

        Destroy(wall);
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
