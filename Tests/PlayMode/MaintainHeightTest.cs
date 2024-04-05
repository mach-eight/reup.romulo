using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;

using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.dependencyInjectors;
using ReupVirtualTwin.managerInterfaces;


public class MaintainHeightTest : MonoBehaviour
{
    GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/Character.prefab");
    GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Platform.prefab");
    GameObject character;
    GameObject widePlatform;

    float HEIGHT_CLOSENESS_THRESHOLD = 0.02f;

    [SetUp]
    public void SetUp()
    {
        StubOnSetupBuildingCreator.CreateOnSetupBuilding();
        character = (GameObject)PrefabUtility.InstantiatePrefab(characterPrefab);
        DestroyGameRelatedDependecyInjectors();
        var posManager = character.GetComponent<ICharacterPositionManager>();
        posManager.maxStepHeight = 0.25f;
        widePlatform = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
        SetPlatform();
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(character);
        Destroy(widePlatform);
    }

    [UnityTest]
    public IEnumerator CharacterShouldFallToDesiredHeight()
    {
        character.transform.position = new Vector3(0, 4, 0);
        yield return new WaitForSeconds(1);
        var distanceToDesiredHeight = MaintainHeight.GetDesiredHeightInGround(0) - character.transform.position.y;
        Assert.LessOrEqual(distanceToDesiredHeight, HEIGHT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator CharacterShouldRiseToDesiredHeight()
    {
        character.transform.position = new Vector3(0, 1.5f, 0);
        yield return new WaitForSeconds(0.2f);
        var distanceToDesiredHeight = MaintainHeight.GetDesiredHeightInGround(0) - character.transform.position.y;
        Assert.LessOrEqual(distanceToDesiredHeight, HEIGHT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator CharacterShouldRiseToDesiredHeightWhenPlatformRises()
    {
        character.transform.position = new Vector3(0, 1.5f, 0);
        yield return new WaitForSeconds(0.2f);
        var upDistance = 0.1f;
        widePlatform.transform.position = new Vector3(0, upDistance, 0);
        yield return new WaitForSeconds(0.2f);
        var distanceToDesiredHeight = MaintainHeight.GetDesiredHeightInGround(0) + upDistance - character.transform.position.y;
        Assert.LessOrEqual(distanceToDesiredHeight, HEIGHT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator CharacterShouldNotRiseIfDistanceIsTooBig()
    {
        character.transform.position = new Vector3(0, 1.5f, 0);
        yield return new WaitForSeconds(0.2f);
        var upDistance = 0.3f;
        widePlatform.transform.position = new Vector3(0, upDistance, 0);
        yield return new WaitForSeconds(0.1f);
        var distanceToDesiredHeight = MaintainHeight.GetDesiredHeightInGround(0) - character.transform.position.y;
        Assert.LessOrEqual(distanceToDesiredHeight, HEIGHT_CLOSENESS_THRESHOLD);
    }

    private void SetPlatform()
    {
        widePlatform.transform.localScale = new Vector3(10, 0.1f, 10);
        widePlatform.transform.position = new Vector3(0, -0.05f, 0);
    }
    private void DestroyGameRelatedDependecyInjectors()
    {
        var movementSelectPosDependencyInjector = character.transform.Find("Behaviours").Find("PointerMovement").GetComponent<CharacterMovementSelectPositionDependenciesInjector>();
        Destroy(movementSelectPosDependencyInjector);
        var initialSpawnDependencyInjector = character.transform.Find("Behaviours").Find("HeightMediator").Find("InitialSpawn").GetComponent<InitialSpawnDependencyInjector>();
        Destroy(initialSpawnDependencyInjector);

    }
}
