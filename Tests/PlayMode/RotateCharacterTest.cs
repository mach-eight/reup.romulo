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
        Transform characterTransform;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;
        float zeroThreshold = 1e-5f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
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
            Debug.Log("start Action done");
            for (int i = 1; i < steps + 1; i++)
            {
                float prevT = (float)(i-1) / steps;
                float t = (float)i / steps;
                Vector2 prevPostion = Vector2.Lerp(startPosition, endPosition, prevT);
                Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, t);
                Vector2 delta = currentPosition - prevPostion;
                stepAction(currentPosition, delta);
                Debug.Log("step action done");
                yield return null;
            }
            endAction(endPosition);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RotateHorizontallyWithTouch()
        {
            AssertVectorIsZero(characterTransform.eulerAngles);
            int touchId = 0;
            yield return MoveFinger(touchId, new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.5f), 90);
            bool characterTurnedLeft = (characterTransform.eulerAngles.y > 180 && characterTransform.eulerAngles.y < 360) ||
                (characterTransform.eulerAngles.y < 0 && characterTransform.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(characterTransform.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(characterTransform.eulerAngles.z, zeroThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RotateHorizontallyWithMouse()
        {
            AssertVectorIsZero(characterTransform.eulerAngles);
            yield return DragMouseLeftButton(new Vector2(0.5f, 0.5f), new Vector2(0.3f, 0.5f), 90);
            bool characterTurnedLeft = (characterTransform.eulerAngles.y > 180 && characterTransform.eulerAngles.y < 360) ||
                (characterTransform.eulerAngles.y < 0 && characterTransform.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(characterTransform.eulerAngles.x, zeroThreshold);
            Assert.LessOrEqual(characterTransform.eulerAngles.z, zeroThreshold);
            yield return null;
        }

    }
}
