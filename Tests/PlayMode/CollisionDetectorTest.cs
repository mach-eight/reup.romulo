using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;

using ReupVirtualTwin.characterMovement;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.behaviourInterfaces;
using Packages.reup.romulo.Tests.PlayMode.Mocks;

public class CollisionDetectorTest : MonoBehaviour
{
    GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/Character.prefab");
    GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Cube.prefab");
    GameObject character;
    GameObject widePlatform;
    GameObject wall;
    CharacterPositionManager posManager;
    private InitialSpawn initialSpawn;

    [SetUp]
    public void SetUp()
    {
        character = (GameObject)PrefabUtility.InstantiatePrefab(characterPrefab);
        DestroyGameRelatedDependecyInjectors();
        posManager = character.GetComponent<CharacterPositionManager>();
        widePlatform = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        SetPlatform();
        initialSpawn = character.transform.Find("Behaviours").Find("HeightMediator").Find("MaintainHeight").GetComponent<InitialSpawn>();
        MockSetUpBuilding mockSetUpBuilding = new MockSetUpBuilding();
        initialSpawn.setUpBuilding = mockSetUpBuilding;
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(character);
        Destroy(widePlatform);
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
    private void DestroyGameRelatedDependecyInjectors()
    {
        var movementSelectPosDependencyInjector = character.transform.Find("Behaviours").Find("PointerMovement").GetComponent<CharacterMovementSelectPositionDependenciesInjector>();
        Destroy(movementSelectPosDependencyInjector);
        var initalSpawnDependencyInjector = character.transform.Find("Behaviours").Find("HeightMediator").Find("MaintainHeight").GetComponent<InitialSpawnDependencyInjector>();
        Destroy(initalSpawnDependencyInjector);

    }
}
