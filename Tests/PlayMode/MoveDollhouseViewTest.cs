using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;


namespace ReupVirtualTwinTests.behaviours
{
    public class MoveDollhouseViewTest : MonoBehaviour
    {
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform dollhouseViewWrapper;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;
        float moveSpeedMetresPerSecond;
        float timeInSecsForHoldingButton = 0.25f;
        float errorToleranceInMeters = 0.1f;
        int pointerSteps = 10;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            dollhouseViewWrapper = sceneObjects.dollhouseViewWrapper;
            moveSpeedMetresPerSecond = MoveDhvCamera.KEYBOARD_MOVE_CAMERA_SPEED_METERS_PER_SECOND;
            sceneObjects.viewModeManager.ActivateDHV();
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [Test]
        public void MoveDhvCameraSpeedIsDefined()
        {
            Assert.AreEqual(10, moveSpeedMetresPerSecond);
        }

        [UnityTest]
        public IEnumerator WKeyShouldMoveDHVForward()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            input.Press(keyboard.wKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.wKey);
            Assert.GreaterOrEqual(dollhouseViewWrapper.position.z, moveSpeedMetresPerSecond * timeInSecsForHoldingButton);
            Assert.Zero(dollhouseViewWrapper.position.x);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AKeyShouldMoveDHVCameraToTheLeft()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            input.Press(keyboard.aKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.aKey);
            Assert.LessOrEqual(dollhouseViewWrapper.position.x, -1 * moveSpeedMetresPerSecond * timeInSecsForHoldingButton);
            Assert.Zero(dollhouseViewWrapper.position.z);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DKeyShouldMoveDHVCameraToTheRight()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            input.Press(keyboard.dKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.dKey);
            Assert.GreaterOrEqual(dollhouseViewWrapper.position.x, moveSpeedMetresPerSecond * timeInSecsForHoldingButton);
            Assert.Zero(dollhouseViewWrapper.position.z);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SKeyShouldMoveDHVCameraBackwards()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            input.Press(keyboard.sKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.sKey);
            Assert.LessOrEqual(dollhouseViewWrapper.position.z, -1 * moveSpeedMetresPerSecond * timeInSecsForHoldingButton);
            Assert.Zero(dollhouseViewWrapper.position.x);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotMoveSidewaysWithMouseWhenNoDragging()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            yield return MovePointerUtils.MoveMouse(input, mouse, Vector2.zero, new Vector2(1,1), pointerSteps);
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveSidewaysWithMouse()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            float relativeMovement = 0.4f;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.5f + relativeMovement, 0.5f);
            yield return MovePointerUtils.DragMouseLeftButton(input, mouse, initialPosition, finalPosition, pointerSteps);
            float expectedMovement = -1 * relativeMovement * MoveDhvCamera.POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT;
            Assert.LessOrEqual(dollhouseViewWrapper.position.x, expectedMovement + errorToleranceInMeters);
            Assert.Zero(dollhouseViewWrapper.position.z);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveForwardWithMouse()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            float relativeMovement = 0.4f;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.5f, relativeMovement + 0.5f);
            yield return new WaitForSeconds(15);
            yield return MovePointerUtils.DragMouseLeftButton(input, mouse, initialPosition, finalPosition, pointerSteps);
            yield return new WaitForSeconds(15);
            float expectedMovement = -1 * relativeMovement * MoveDhvCamera.POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT;
            Assert.Zero(dollhouseViewWrapper.position.x);
            Assert.Zero(dollhouseViewWrapper.position.y);
            Assert.GreaterOrEqual(dollhouseViewWrapper.position.z, expectedMovement - errorToleranceInMeters);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveSidewaysWithTouch()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            float relativeMovement = 0.4f;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.5f + relativeMovement, 0.5f);
            int touchId = 0;
            yield return MovePointerUtils.MoveFinger(input, touch, touchId, initialPosition, finalPosition, pointerSteps);
            float expectedMovement = -1 * relativeMovement * MoveDhvCamera.POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT;
            Assert.LessOrEqual(dollhouseViewWrapper.position.x, expectedMovement + errorToleranceInMeters);
            Assert.Zero(dollhouseViewWrapper.position.z);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveForwardWithTouch()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            float relativeMovement = 0.4f;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.5f, relativeMovement + 0.5f);
            int touchId = 0;
            yield return MovePointerUtils.MoveFinger(input, touch, touchId, initialPosition, finalPosition, pointerSteps);
            float expectedMovement = -1 * relativeMovement * MoveDhvCamera.POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT;
            Assert.Zero(dollhouseViewWrapper.position.x);
            Assert.GreaterOrEqual(dollhouseViewWrapper.position.z, expectedMovement - errorToleranceInMeters);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

    }
}
