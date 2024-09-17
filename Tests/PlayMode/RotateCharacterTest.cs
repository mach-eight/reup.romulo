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
    public class RotateCharacterTest
    {
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform characterTransform;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;

        float zeroThreshold = 1e-5f;

        //float timeInSecsForEachAction = 0.25f;

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

        IEnumerator MoveFinger(int touchId, Vector2 startScreenPosition, Vector2 endScreenPosition, int steps)
        {
            Vector2 touchStartPosition = Camera.main.ViewportToScreenPoint(startScreenPosition);
            Vector2 touchEndPosition = Camera.main.ViewportToScreenPoint(endScreenPosition);
            input.SetTouch(touchId, InSys.TouchPhase.Began, touchStartPosition, Vector2.zero, true, touch);
            for (int i = 1; i < steps + 1; i++)
            {
                float prevT = (float)(i-1) / steps;
                float t = (float)i / steps;
                Vector2 prevPostion = Vector2.Lerp(touchStartPosition, touchEndPosition, prevT);
                Vector2 currentPosition = Vector2.Lerp(touchStartPosition, touchEndPosition, t);
                Vector2 delta = currentPosition - prevPostion;
                input.SetTouch(touchId, InSys.TouchPhase.Moved, currentPosition, delta, true, touch);
                yield return null;
            }
            input.EndTouch(touchId, touchEndPosition, Vector2.zero, true, touch);
        }

        [UnityTest]
        public IEnumerator RotateHorizontallyWithTouch()
        {
            AssertVectorIsZero(characterTransform.eulerAngles);
            int touchId = 0;
            yield return MoveFinger(touchId, new Vector2(0.5f, 0.5f), new Vector2(0.9f, 0.5f), 90);
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
            Vector2 mouseStartPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
            Vector2 touchEndPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.9f, 0.5f, 0));

            //int touchId = 0;
            //InSys.TouchPhase phase = InSys.TouchPhase.Began;
            //Debug.Log("set touch");
            //input.SetTouch(touchId, phase, touchStartPosition, 1.0f, Vector2.zero, true, touch);
            input.Move(mouse.position, mouseStartPosition);
            input.Press(mouse.leftButton);
            //Debug.Log("touch set");
            //float duration = 1.5f;
            int steps = 90;
            //float timeStep = duration / steps;
            //Debug.Log("timeStep");
            //Debug.Log(timeStep);

            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / steps;
                //Debug.Log("t");
                //Debug.Log(t);
                Vector2 currentPosition = Vector2.Lerp(mouseStartPosition, touchEndPosition, t);
                //Debug.Log("currentPosition");
                //Debug.Log(currentPosition);
                //input.MoveTouch(touchId, currentPosition);
                input.Move(mouse.position, currentPosition);
                yield return null;
                //yield return new WaitForSeconds(timeStep);
            }

            //input.EndTouch(touchId, touchEndPosition);
            input.Release(mouse.leftButton);

            yield return new WaitForSeconds(2);
        }

    }
}
