using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

using ReupVirtualTwin.dependencyInjectors;
using ReupVirtualTwin.managers;

public class CharacterPositionManagerTest : MonoBehaviour
{
    private GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/Character.prefab");
    private GameObject character;
    private CharacterPositionManager posManager;
    private GameObject setupBuildingGameObject;

    float HEIGHT_CLOSENESS_THRESHOLD = 0.02f;
    float MOVEMENT_CLOSENESS_THRESHOLD = 0.02f;
    float WALK_CLOSENESS_THRESHOLD = 0.5f;

    [SetUp]
    public void SetUp()
    {
        setupBuildingGameObject = StubOnSetupBuildingCreator.CreateImmediateOnSetupBuilding();
        character = (GameObject)PrefabUtility.InstantiatePrefab(characterPrefab);
        DestroyGameRelatedDependecyInjectors();
        character.transform.position = Vector3.zero;
        posManager = character.GetComponent<CharacterPositionManager>();
        posManager.maxStepHeight = 0.25f;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Destroy(character);
        Destroy(setupBuildingGameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SlideToTargetShouldSuccess()
    {
        //check original character's position
        Assert.AreEqual(Vector3.zero, character.transform.position);

        var targetPosition = new Vector3(5, 5, 5);

        posManager.SlideToTarget(targetPosition);

        yield return new WaitForSeconds(3);

        //check new character's position is REALLY close to target position
        Assert.LessOrEqual(Vector3.Distance(targetPosition, character.transform.position), MOVEMENT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator WalkToTargetShouldSuccess()
    {
        //check original character's position
        Assert.AreEqual(Vector3.zero, character.transform.position);

        var targetPosition = new Vector3(5, 5, 5);

        posManager.WalkToTarget(targetPosition);

        yield return new WaitForSeconds(6);

        var sameHeightTargetPosition = new Vector3(5, 0, 5);

        //check new character's position is close to target position
        Assert.LessOrEqual(Vector3.Distance(sameHeightTargetPosition, character.transform.position), WALK_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator HeightToTargetShouldSuccessIfFallingDown()
    {
        //check original character's position
        Assert.AreEqual(character.transform.position, Vector3.zero);

        posManager.KeepHeight(-1f);

        yield return new WaitForSeconds(1);

        //check new character's position is close to target position
        var expectedPosition = new Vector3(0, -1, 0);
        Assert.LessOrEqual(Vector3.Distance(character.transform.position, expectedPosition), HEIGHT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator HeightToTargetShouldSuccessIfStepingUp()
    {
        //check original character's position
        Assert.AreEqual(Vector3.zero, character.transform.position);

        posManager.KeepHeight(0.20f);

        yield return new WaitForSeconds(1);

        //check new character's position is close to target position
        var expectedPosition = new Vector3(0, 0.2f, 0);
        Assert.LessOrEqual(Vector3.Distance(character.transform.position, expectedPosition), HEIGHT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator HeightToTargetShouldFailIfStepingTooMuchUp()
    {
        //check original character's position
        Assert.AreEqual(character.transform.position, Vector3.zero);

        posManager.KeepHeight(0.35f);

        yield return new WaitForSeconds(1);

        //check new character's position is close to target position
        var expectedPosition = Vector3.zero;
        Assert.LessOrEqual(Vector3.Distance(character.transform.position, expectedPosition), HEIGHT_CLOSENESS_THRESHOLD);
    }
    [UnityTest]
    public IEnumerator MoveDistanceInDirectionShouldSuccess()
    {
        //check original character's position
        Assert.AreEqual(Vector3.zero, character.transform.position);

        var direction = new Vector3(1, 1, 2);
        posManager.MoveDistanceInDirection(2f, direction);

        //check new character's position is close to target position
        var sqrt6 = Mathf.Sqrt(6);
        var expectedPosition = new Vector3(sqrt6, sqrt6, 2 * sqrt6) / 3;
        Assert.LessOrEqual(Vector3.Distance(character.transform.position, expectedPosition), 1E-5);
        yield return null;
    }
    private void DestroyGameRelatedDependecyInjectors()
    {
        var movementSelectPosDependencyInjector = character.transform.Find("Behaviours").Find("PointerMovement").GetComponent<CharacterMovementSelectPositionDependenciesInjector>();
        var initialSpawnDependencyInjector = character.transform.Find("Behaviours").Find("HeightMediator").Find("InitialSpawn").GetComponent<InitialSpawnDependencyInjector>();
        Destroy(movementSelectPosDependencyInjector);
        Destroy(initialSpawnDependencyInjector);
    }
}

