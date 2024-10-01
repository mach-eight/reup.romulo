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
        float limitFromBuildingInMeters;
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
            limitFromBuildingInMeters = sceneObjects.moveDhvCameraBehavior.limitDistanceFromBuildingInMeters;
            dollhouseViewWrapper = sceneObjects.dollhouseViewWrapper;
            moveSpeedMetresPerSecond = sceneObjects.moveDhvCameraBehavior.KeyboardMoveCameraSpeedMetersPerSecond;
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
            Assert.AreEqual(40, moveSpeedMetresPerSecond);
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
            float expectedMovement = -1 * relativeMovement * sceneObjects.moveDhvCameraBehavior.PointerMoveCameraDistanceInMetersSquareViewport;
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
            Vector2 finalPosition = new Vector2(0.5f, 0.5f - relativeMovement);
            yield return MovePointerUtils.DragMouseLeftButton(input, mouse, initialPosition, finalPosition, pointerSteps);
            float expectedMovement = relativeMovement * sceneObjects.moveDhvCameraBehavior.PointerMoveCameraDistanceInMetersSquareViewport;
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
            float expectedMovement = -1 * relativeMovement * sceneObjects.moveDhvCameraBehavior.PointerMoveCameraDistanceInMetersSquareViewport;
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
            Vector2 finalPosition = new Vector2(0.5f, 0.5f - relativeMovement);
            int touchId = 0;
            yield return MovePointerUtils.MoveFinger(input, touch, touchId, initialPosition, finalPosition, pointerSteps);
            float expectedMovement = -1 * relativeMovement * sceneObjects.moveDhvCameraBehavior.PointerMoveCameraDistanceInMetersSquareViewport;
            Assert.Zero(dollhouseViewWrapper.position.x);
            Assert.GreaterOrEqual(dollhouseViewWrapper.position.z, expectedMovement - errorToleranceInMeters);
            Assert.Zero(dollhouseViewWrapper.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldStopMovementWhenBoundariesAreReached()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);

            float extraDistanceInMeters = 5;
            float movementDistanceBeyondBoundaries = limitFromBuildingInMeters + extraDistanceInMeters;
            float timeToMoveBeyondBoundaries = movementDistanceBeyondBoundaries / moveSpeedMetresPerSecond;
            
            input.Press(keyboard.wKey);
            yield return new WaitForSeconds(timeToMoveBeyondBoundaries);
            input.Release(keyboard.wKey);

            Vector3 expectedFinalPosition = new Vector3(0, 0, limitFromBuildingInMeters);

            Assert.LessOrEqual(dollhouseViewWrapper.position.z, expectedFinalPosition.z);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotMoveWhenGesturesInProgress()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);

            Vector2 startFinger1 = new Vector2(200, 200);
            Vector2 startFinger2 = new Vector2(400, 400);
            Vector2 endFinger1 = new Vector2(100, 100);
            Vector2 endFinger2 = new Vector2(500, 500);
            yield return MovePointerUtils.TouchGesture(input, touch, startFinger1, startFinger2, endFinger1, endFinger2, 10);

            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            yield return null;
        }
    }
}
