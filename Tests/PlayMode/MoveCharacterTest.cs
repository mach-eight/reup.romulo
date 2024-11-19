using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using InSys = UnityEngine.InputSystem;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEditor;
using ReupVirtualTwinTests.instantiators;

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

        float timeInSecsForHoldingButton = 0.5f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(frontWallMock);
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            characterTransform = sceneObjects.character;
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
            Assert.AreEqual(2.5f, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND);
        }

        [UnityTest]
        public IEnumerator WKeyShouldMoveCharacterForward()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.wKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.wKey);
            float lowerMovementLimit = 1;
            float upperMovementLimit = 2;
            Assert.GreaterOrEqual(characterTransform.position.z, lowerMovementLimit);
            Assert.LessOrEqual(characterTransform.position.z, upperMovementLimit);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AKeyShouldMoveCharacterToTheLeft()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.aKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.aKey);
            float lowerMovementLimit = -2;
            float upperMovementLimit = -1;
            Assert.LessOrEqual(characterTransform.position.x, upperMovementLimit);
            Assert.GreaterOrEqual(characterTransform.position.x, lowerMovementLimit);
            Assert.Zero(characterTransform.position.z);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DKeyShouldMoveCharacterToTheRight()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.dKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.dKey);
            float lowerMovementLimit = 1;
            float upperMovementLimit = 2;
            Assert.GreaterOrEqual(characterTransform.position.x, lowerMovementLimit);
            Assert.LessOrEqual(characterTransform.position.x, upperMovementLimit);
            Assert.Zero(characterTransform.position.z);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SKeyShouldMoveCharacterBackwards()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.sKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.sKey);
            float lowerMovementLimit = -2;
            float upperMovementLimit = -1;
            Assert.LessOrEqual(characterTransform.position.z, upperMovementLimit);
            Assert.GreaterOrEqual(characterTransform.position.z, lowerMovementLimit);
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
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.upArrowKey);
            float lowerMovementLimit = 1;
            float upperMovementLimit = 2;
            Assert.GreaterOrEqual(characterTransform.position.z, lowerMovementLimit);
            Assert.LessOrEqual(characterTransform.position.z, upperMovementLimit);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DownArrowKeyShouldMoveCharacterBackwards()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.downArrowKey);
            float lowerMovementLimit = -2;
            float upperMovementLimit = -1;
            Assert.LessOrEqual(characterTransform.position.z, upperMovementLimit);
            Assert.GreaterOrEqual(characterTransform.position.z, lowerMovementLimit);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

    }
}
