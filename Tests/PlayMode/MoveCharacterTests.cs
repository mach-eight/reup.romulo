using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEditor;

namespace ReupVirtualTwinTests.behaviours
{
    public class MoveCharacterTests
    {
        GameObject frontWallMock = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/FrontWallMock.prefab");
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Keyboard keyboard;
        Mouse mouse;
        Transform characterTransform;

        float timeInSecsForEachAction = 0.25f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(frontWallMock);
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            characterTransform = sceneObjects.character.transform;
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [Test]
        public void WalkSpeedIsDefined()
        {
            Assert.AreEqual(3.5f, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND);
        }

        [UnityTest]
        public IEnumerator WKeyShouldMoveCharacterForward()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.wKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.wKey);
            Assert.GreaterOrEqual(characterTransform.position.z, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AKeyShouldMoveCharacterToTheLeft()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.aKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.aKey);
            Assert.LessOrEqual(characterTransform.position.x, -1 * CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.z);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DKeyShouldMoveCharacterToTheRight()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.dKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.dKey);
            Assert.GreaterOrEqual(characterTransform.position.x, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.z);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SKeyShouldMoveCharacterBackwards()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.sKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.sKey);
            Assert.LessOrEqual(characterTransform.position.z, -1 * CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ClickInObjectMoveCharacterToObject()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            var desiredMousePositionInScreen = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
            input.Move(mouse.position, new Vector2(desiredMousePositionInScreen.x, desiredMousePositionInScreen.y));
            input.PressAndRelease(mouse.leftButton);
            yield return new WaitForSeconds(2);
            Assert.GreaterOrEqual(characterTransform.position.z, 4.5f); // Wall is at 5 meters away, so it should stop at 4.5
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
        }

        [UnityTest]
        public IEnumerator UpArrowKeyShouldMoveCharacterForward()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.upArrowKey);
            Assert.GreaterOrEqual(characterTransform.position.z, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DownArrowKeyShouldMoveCharacterBackwards()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.downArrowKey);
            Assert.LessOrEqual(characterTransform.position.z, -1 * CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

    }
}
