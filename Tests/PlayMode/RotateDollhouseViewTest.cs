using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.helpers;
using ReupVirtualTwinTests.utils;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.behaviours
{
    public class RotateDollhouseViewTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        InputTestFixture input;
        Transform dollhouseViewWrapper;
        RotateDhvCameraKeyboard rotateDhvCameraKeyboardBehavior;
        RotateDhvCameraMouse rotateDhvCameraMouseBehavior;
        RotateDhvCameraTouch rotateDhvCameraTouchBehavior;
        CinemachineCamera dhvCineMachineComponent;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;

        private float initialRotationX;
        private float initialRotationY;
        private float keyboardRotationSpeedDegreesPerSecond;
        private float timeInSecsForHoldingButton = 0.25f;
        private float errorToleranceInDegrees = 2.5f;
        private const int steps = 10;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            dollhouseViewWrapper = sceneObjects.dollhouseViewWrapper;
            rotateDhvCameraKeyboardBehavior = sceneObjects.rotateDhvCameraKeyboardBehavior;
            rotateDhvCameraMouseBehavior = sceneObjects.rotateDhvCameraMouseBehavior;
            rotateDhvCameraTouchBehavior = sceneObjects.rotateDhvCameraTouchBehavior;
            dhvCineMachineComponent = sceneObjects.dhvCamera.GetComponent<CinemachineCamera>();
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            initialRotationX = dollhouseViewWrapper.localEulerAngles.x;
            initialRotationY = dollhouseViewWrapper.localEulerAngles.y;
            keyboardRotationSpeedDegreesPerSecond = rotateDhvCameraKeyboardBehavior.keyboardRotationSpeedDegreesPerSecond;
            sceneObjects.viewModeManager.ActivateDHV();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [UnityTest]
        public IEnumerator UpArrowKeyShouldRotateDHVUp()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            input.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.upArrowKey);
            yield return null;

            float expectedRotation = MathUtils.NormalizeTo360Degrees(initialRotationX + (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator DownArrowKeyShouldRotateDHVDown()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            input.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.downArrowKey);
            yield return null;

            float expectedRotation = MathUtils.NormalizeTo360Degrees(initialRotationX - (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator LeftArrowKeyShouldRotateDHVLeft()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            input.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.leftArrowKey);
            yield return null;

            float expectedRotation = MathUtils.NormalizeTo360Degrees(initialRotationY + (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator RightArrowKeyShouldRotateDHVRight()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            input.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.rightArrowKey);
            yield return null;

            float expectedRotation = MathUtils.NormalizeTo360Degrees(initialRotationY - (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMaxVerticalRotationAngleWithKeyboard()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float angleToClamp = RomuloGlobalSettings.maxVerticalAngle + 5;
            float timeToHoldHeyBeyondClamp = Mathf.Abs((angleToClamp - initialRotationX)) / keyboardRotationSpeedDegreesPerSecond;

            input.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(timeToHoldHeyBeyondClamp);
            input.Release(keyboard.upArrowKey);

            Assert.AreEqual(RomuloGlobalSettings.maxVerticalAngle, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMinVerticalRotationAngleWithKeyboard()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float angleToClamp = RomuloGlobalSettings.minVerticalAngle - 5;
            float timeToHoldHeyBeyondClamp = Mathf.Abs((angleToClamp - initialRotationX)) / keyboardRotationSpeedDegreesPerSecond;

            input.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(timeToHoldHeyBeyondClamp);
            input.Release(keyboard.downArrowKey);

            Assert.AreEqual(RomuloGlobalSettings.minVerticalAngle, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldNotRotateDHVWithMouseWhenNoDraggingRightButton()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseLeftButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.7f), steps);

            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);
        }   

        [UnityTest]
        public IEnumerator ShouldRotate9DegreesUpDHVWithMouseWhenDraggingRightButtonDown()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * RomuloGlobalSettings.verticalRotationPerScreenHeight;
            Vector2 initialPosition = new Vector2(0.0f, 1.0f);
            Vector2 finalPosition = new Vector2(0.0f, 0.95f);

            yield return PointerUtils.DragMouseRightButton(input, mouse, initialPosition, finalPosition, steps);

            float expectedRotation = initialRotationX + performedRotationInDegrees;

            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate9DegreesDownWithMouseWhenDraggingRightButtonUp()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * RomuloGlobalSettings.verticalRotationPerScreenHeight;
            Vector2 initialPosition = new Vector2(0.0f, 0.0f);
            Vector2 finalPosition = new Vector2(0.0f, 0.05f);

            yield return PointerUtils.DragMouseRightButton(input, mouse, initialPosition, finalPosition, steps);

            float expectedRotation = initialRotationX - performedRotationInDegrees;

            Assert.AreEqual(dollhouseViewWrapper.localEulerAngles.x, expectedRotation, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate9DegreesLeftWithMouseWhenDraggingRightButtonRight()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * RomuloGlobalSettings.horizontalRotationPerScreenWidth;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.45f, 0.5f);

            yield return PointerUtils.DragMouseRightButton(input, mouse, initialPosition, finalPosition, steps);

            float expectedRotation = 360 - performedRotationInDegrees;

            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate9DegreesRightWithMouseWhenDraggingRightButtonLeft()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * RomuloGlobalSettings.horizontalRotationPerScreenWidth;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.55f, 0.5f);

            yield return PointerUtils.DragMouseRightButton(input, mouse, initialPosition, finalPosition, steps);

            float expectedRotation = initialRotationY + performedRotationInDegrees;

            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMaxVerticalRotationAngleWithMouse()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, -100.0f), steps);

            Assert.AreEqual(RomuloGlobalSettings.maxVerticalAngle, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMinVerticalRotationAngleWithMouse()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 100.0f), steps);

            Assert.AreEqual(RomuloGlobalSettings.minVerticalAngle, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldNotRotateWithMultiTouchGestureWhenAngleDoesNotChange()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            Vector2 initialPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.0f, 0.5f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 finalPositionTouch2 = new Vector2(0.0f, 0.10f);

            yield return PointerUtils.AbsolutePositionTouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            float actualRotationX = dollhouseViewWrapper.localEulerAngles.x;
            float actualRotationY = dollhouseViewWrapper.localEulerAngles.y;

            Assert.AreEqual(initialRotationX, actualRotationX, errorToleranceInDegrees);
            Assert.AreEqual(initialRotationY, actualRotationY, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate45DegreesCounterClockWiseWithMultiTouch()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float performedRotation = 45;
            Vector2 initialPositionTouch1 = new Vector2(1.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(1.0f, 0.5f);
            Vector2 finalPositionTouch1 = new Vector2(1.0f, 0.0f);
            Vector2 finalPositionTouch2 = new Vector2(1.5f, 0.5f);

            yield return PointerUtils.AbsolutePositionTouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);
            
            float expectedRotationY = 360 - performedRotation; // 360 -> Because we are rotating counter clockwise
            
            Assert.AreEqual(expectedRotationY, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate45DegreesClockwiseWithMultiTouchGesture()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float performedRotation = 45;
            Vector2 initialPositionTouch1 = new Vector2(1.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(1.0f, 0.5f);
            Vector2 finalPositionTouch1 = new Vector2(1.0f, 0.0f);
            Vector2 finalPositionTouch2 = new Vector2(0.5f, 0.5f);

            yield return PointerUtils.AbsolutePositionTouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);
            
            float expectedRotationY = initialRotationY + performedRotation;

            Assert.AreEqual(expectedRotationY, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldNotRotateVerticallyWithOpposingTouches()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            Vector2 initialPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.0f, 0.5f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 finalPositionTouch2 = new Vector2(0.5f, 0.0f);

            yield return PointerUtils.AbsolutePositionTouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate9DegreesVerticallyUpWithAlignedTouches()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * 180;
            Vector2 initialPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.1f, 0.0f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 0.05f);
            Vector2 finalPositionTouch2 = new Vector2(0.1f, 0.05f);

            yield return PointerUtils.TouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            float expectedRotationX = initialRotationX - performedRotationInDegrees;

            Assert.AreEqual(expectedRotationX, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldRotate9DegreesVerticallyDownWithAlignedTouches()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * RomuloGlobalSettings.verticalRotationPerScreenHeight;
            Vector2 initialPositionTouch1 = new Vector2(0.0f, 1.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.1f, 1.0f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 0.95f);
            Vector2 finalPositionTouch2 = new Vector2(0.1f, 0.95f);

            yield return PointerUtils.TouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            float expectedRotationX = initialRotationX + performedRotationInDegrees;

            Assert.AreEqual(expectedRotationX, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMaxVerticalRotationAngleWithMultiTouchGesture()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            Vector2 initialPositionTouch1 = new Vector2(0.0f, 1.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.1f, 1.0f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 finalPositionTouch2 = new Vector2(0.1f, 0.0f);

            yield return PointerUtils.TouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            Assert.AreEqual(RomuloGlobalSettings.maxVerticalAngle, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMinVerticalRotationAngleWithMultiTouchGesture()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            Vector2 initialPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.0f, 0.0f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 1.0f);
            Vector2 finalPositionTouch2 = new Vector2(0.0f, 1.0f);

            yield return PointerUtils.TouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            Assert.AreEqual(RomuloGlobalSettings.minVerticalAngle, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldNotZoomWhileRotationTouchMultiGesture()
        {
            float initialFieldOfView = dhvCineMachineComponent.Lens.FieldOfView;

            Vector2 initialPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 initialPositionTouch2 = new Vector2(0.0f, 0.5f);
            Vector2 finalPositionTouch1 = new Vector2(0.0f, 0.0f);
            Vector2 finalPositionTouch2 = new Vector2(0.0f, 0.10f);

            yield return PointerUtils.AbsolutePositionTouchGesture(input, touch, initialPositionTouch1, initialPositionTouch2, finalPositionTouch1, finalPositionTouch2, steps);

            Assert.AreEqual(initialFieldOfView, dhvCineMachineComponent.Lens.FieldOfView);
        }

        [UnityTest]
        public IEnumerator ShouldOnlyRotateIfBothMouseButtonsAreHold()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);

            float percentageOfTheScreenDragged = 5f / 100f;
            float performedRotationInDegrees = percentageOfTheScreenDragged * RomuloGlobalSettings.horizontalRotationPerScreenWidth;
            Vector2 initialPosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPosition = new Vector2(0.55f, 0.5f);

            yield return PointerUtils.DragMouseBothButtons(input, mouse, initialPosition, finalPosition, steps);

            float expectedRotation = initialRotationY + performedRotationInDegrees;

            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
        }

    }
}
