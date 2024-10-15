using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.managers;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.managerInterfaces;

public class CharacterPositionManagerTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;
    Transform character;

    CharacterPositionManager posManager;

    float HEIGHT_CLOSENESS_THRESHOLD = 0.02f;
    float MOVEMENT_CLOSENESS_THRESHOLD = 0.02f;
    float WALK_CLOSENESS_THRESHOLD = 0.5f;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        sceneObjects = ReupSceneInstantiator.InstantiateScene();
        character = sceneObjects.character;
        // posManager = character.GetComponent<CharacterPositionManager>();
        posManager = sceneObjects.diContainer.Resolve<ICharacterPositionManager>() as CharacterPositionManager;
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SlideToTargetShouldSuccess()
    {
        //check original character's position
        Assert.AreEqual(Vector3.zero, character.transform.position);

        var targetPosition = new Vector3(1, 1, 1);

        posManager.SlideToTarget(targetPosition);

        yield return new WaitForSeconds(1);

        //check new character's position is REALLY close to target position
        Assert.LessOrEqual(Vector3.Distance(targetPosition, character.transform.position), MOVEMENT_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator WalkToTargetShouldSuccess()
    {
        //check original character's position
        Assert.AreEqual(Vector3.zero, character.transform.position);

        var targetPosition = new Vector3(1, 5, 1);

        posManager.WalkToTarget(targetPosition);

        yield return new WaitForSeconds(1);

        var sameHeightTargetPosition = new Vector3(1, 0, 1);

        //check new character's position is close to target position
        Assert.LessOrEqual(Vector3.Distance(sameHeightTargetPosition, character.transform.position), WALK_CLOSENESS_THRESHOLD);
    }

    [UnityTest]
    public IEnumerator HeightToTargetShouldSuccessIfFallingDown()
    {
        //check original character's position
        Assert.AreEqual(character.transform.position, Vector3.zero);

        yield return null;
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
}

