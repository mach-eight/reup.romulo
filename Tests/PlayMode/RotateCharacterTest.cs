using System;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using InSys = UnityEngine.InputSystem;
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

        void AssertVectorIsZero(Vector3 vector)
        {
            Assert.LessOrEqual(vector.x, zeroThreshold);
            Assert.LessOrEqual(vector.y, zeroThreshold);
            Assert.LessOrEqual(vector.z, zeroThreshold);
        }

        IEnumerator DragMouseLeftButton(Vector2 startMousePoint, Vector2 endMousePoint, int steps)
        {
            return MovePointer(
                startMousePoint,
                endMousePoint,
                steps,
                (Vector2 startPosition) => { input.Move(mouse.position, startPosition); input.Press(mouse.leftButton); },
                (Vector2 currentPosition, Vector2 delta) => {
                    input.Move(mouse.position, currentPosition, delta);
                    input.Set(mouse.delta, delta);
                },
                (Vector2 endPosition) => input.Release(mouse.leftButton)
            );
        }

        IEnumerator MoveFinger(int touchId, Vector2 startScreenPosition, Vector2 endScreenPosition, int steps)
        {
            return MovePointer(
                startScreenPosition,
                endScreenPosition,
                steps,
                (Vector2 startPosition) => input.SetTouch(touchId, InSys.TouchPhase.Began, startPosition, Vector2.zero, true, touch),
                (Vector2 currentPosition, Vector2 delta) => input.SetTouch(touchId, InSys.TouchPhase.Moved, currentPosition, delta, true, touch),
                (Vector2 endPosition) => input.EndTouch(touchId, endPosition, Vector2.zero, true, touch)
            );
        }

        IEnumerator MovePointer(
            Vector2 startScreenPosition,
            Vector2 endScreenPosition,
            int steps,
            Action<Vector2> startAction,
            Action<Vector2, Vector2> stepAction,
            Action<Vector2> endAction
        ) {
            Vector2 startPosition = Camera.main.ViewportToScreenPoint(startScreenPosition);
            Vector2 endPosition = Camera.main.ViewportToScreenPoint(endScreenPosition);
            startAction(startPosition);
            for (int i = 1; i < steps + 1; i++)
            {
                float prevT = (float)(i-1) / steps;
                float t = (float)i / steps;
                Vector2 prevPostion = Vector2.Lerp(startPosition, endPosition, prevT);
                Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, t);
                Vector2 delta = currentPosition - prevPostion;
                stepAction(currentPosition, delta);
                yield return null;
            }
            endAction(endPosition);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyInTouchscreen()
        {
            AssertVectorIsZero(character.eulerAngles);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            int touchId = 0;
            yield return MoveFinger(touchId, new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.5f), 90);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyWithMouse()
        {
            AssertVectorIsZero(character.eulerAngles);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            yield return DragMouseLeftButton(new Vector2(0.5f, 0.5f), new Vector2(0.3f, 0.5f), 90);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyWithMouse()
        {
            AssertVectorIsZero(character.eulerAngles);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            yield return DragMouseLeftButton(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.7f), 90);
            bool characterLookedUp = (innerCharacter.eulerAngles.x < 0 && innerCharacter.eulerAngles.x > -180) ||
                (innerCharacter.eulerAngles.x > 180 && innerCharacter.eulerAngles.x < 360);
            AssertVectorIsZero(character.eulerAngles);
            Assert.IsTrue(characterLookedUp);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyInTouchscreen()
        {
            AssertVectorIsZero(character.eulerAngles);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            int touchId = 0;
            yield return MoveFinger(touchId, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.3f), 90);
            bool characterLookedUp = (innerCharacter.eulerAngles.x < 0 && innerCharacter.eulerAngles.x > -180) ||
                (innerCharacter.eulerAngles.x > 180 && innerCharacter.eulerAngles.x < 360);
            AssertVectorIsZero(character.eulerAngles);
            Assert.IsTrue(characterLookedUp);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingLeftArrowKey()
        {
            AssertVectorIsZero(character.eulerAngles);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            input.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.leftArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingRightArrowKey()
        {
            AssertVectorIsZero(character.eulerAngles);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            input.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.rightArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsFalse(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, zeroThreshold);
            AssertVectorIsZero(innerCharacter.localEulerAngles);
            yield return null;
        }

    }
}
