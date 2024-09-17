using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using InSys = UnityEngine.InputSystem;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEditor;

namespace ReupVirtualTwinTests.behaviours
{
    public class MoveCharacterTest
    {
        GameObject frontWallMock = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/FrontWallMock.prefab");
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform characterTransform;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;

        float timeInSecsForEachAction = 0.25f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(frontWallMock);
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
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
            var mouse = InputSystem.AddDevice<Mouse>();
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            Vector2 desiredMousePositionInScreen = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
            input.Move(mouse.position, desiredMousePositionInScreen);
            input.PressAndRelease(mouse.leftButton);
            yield return new WaitForSeconds(2);
            Assert.GreaterOrEqual(characterTransform.position.z, 4.5f); // Wall is at 5 meters away, so it should stop at 4.5
        }

        [UnityTest]
        public IEnumerator TapInObjectMoveCharacterToObject()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            Vector2 desiredTapPositionInScreen = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
            int touchId = 0;
            InSys.TouchPhase phase = InSys.TouchPhase.Began;
            input.SetTouch(touchId, phase, desiredTapPositionInScreen, 1.0f, Vector2.zero, true, touch);
            input.EndTouch(touchId, desiredTapPositionInScreen);
            yield return new WaitForSeconds(2);
            Assert.GreaterOrEqual(characterTransform.position.z, 4.5f); // Wall is at 5 meters away, so it should stop at 4.5
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
