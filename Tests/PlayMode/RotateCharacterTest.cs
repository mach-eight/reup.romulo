using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;


namespace ReupVirtualTwinTests.behaviours
{
    public class RotateCharacterTest
    {
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform character;
        Transform innerCharacter;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;
        float zeroThreshold = 1e-5f;

        float timeInSecsForHoldingButton = 0.25f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            character = sceneObjects.character;
            innerCharacter = sceneObjects.innerCharacter;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }


        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            int touchId = 0;
            yield return MovePointerUtils.MoveFinger(input, touch, touchId, new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.5f), 90);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyWithMouse()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            yield return MovePointerUtils.DragMouseLeftButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.3f, 0.5f), 90);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyWithMouse()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            yield return MovePointerUtils.DragMouseLeftButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.7f), 90);
            bool characterLookedUp = (innerCharacter.eulerAngles.x < 0 && innerCharacter.eulerAngles.x > -180) ||
                (innerCharacter.eulerAngles.x > 180 && innerCharacter.eulerAngles.x < 360);
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            Assert.IsTrue(characterLookedUp);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            int touchId = 0;
            yield return MovePointerUtils.MoveFinger(input, touch, touchId, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.3f), 90);
            bool characterLookedUp = (innerCharacter.eulerAngles.x < 0 && innerCharacter.eulerAngles.x > -180) ||
                (innerCharacter.eulerAngles.x > 180 && innerCharacter.eulerAngles.x < 360);
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            Assert.IsTrue(characterLookedUp);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingLeftArrowKey()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            input.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.leftArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingRightArrowKey()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            input.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.rightArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsFalse(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, zeroThreshold);
            yield return null;
        }

    }
}
