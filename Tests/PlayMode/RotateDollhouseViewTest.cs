using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
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
        Keyboard keyboard;
        Mouse mouse;

        private float initialRotationX;
        private float initialRotationY;
        private float keyboardRotationSpeedDegreesPerSecond;
        private float maxVerticalAngle = 89.9f;
        private float minVerticalAngle = 10f;
        private float timeInSecsForHoldingButton = 0.25f;
        private float errorToleranceInDegrees = 1.0f;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            dollhouseViewWrapper = sceneObjects.dollhouseViewWrapper;
            rotateDhvCameraKeyboardBehavior = sceneObjects.rotateDhvCameraKeyboardBehavior;
            rotateDhvCameraMouseBehavior = sceneObjects.rotateDhvCameraMouseBehavior;
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
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

        private float NormalizeAngle(float angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;
            return angle;
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

            float expectedRotation = NormalizeAngle(initialRotationX + (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
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

            float expectedRotation = NormalizeAngle(initialRotationX - (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
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

            float expectedRotation = NormalizeAngle(initialRotationY + (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
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

            float expectedRotation = NormalizeAngle(initialRotationY - (keyboardRotationSpeedDegreesPerSecond * timeInSecsForHoldingButton));
            Assert.AreEqual(expectedRotation, dollhouseViewWrapper.localEulerAngles.y, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMaxVerticalRotationAngleWithKeyboard()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float angleToClamp = maxVerticalAngle + 5;
            float timeToHoldHeyBeyondClamp = Mathf.Abs((angleToClamp - initialRotationX)) / keyboardRotationSpeedDegreesPerSecond;

            input.Press(keyboard.upArrowKey);
            yield return new WaitForSeconds(timeToHoldHeyBeyondClamp);
            input.Release(keyboard.upArrowKey);

            Assert.AreEqual(89.9f, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMinVerticalRotationAngleWithKeyboard()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            float angleToClamp = minVerticalAngle - 5;
            float timeToHoldHeyBeyondClamp = Mathf.Abs((angleToClamp - initialRotationX)) / keyboardRotationSpeedDegreesPerSecond;

            input.Press(keyboard.downArrowKey);
            yield return new WaitForSeconds(timeToHoldHeyBeyondClamp);
            input.Release(keyboard.downArrowKey);

            Assert.AreEqual(10f, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldNotRotateDHVWithMouseWhenNoDraggingRightButton()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseLeftButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.7f), 10);

            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);
        }   

        [UnityTest]
        public IEnumerator ShouldRotateUpDHVWithMouseWhenDraggingRightButtonDown()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.3f), 10);

            Assert.Greater(dollhouseViewWrapper.localEulerAngles.x, initialRotationX);
        }

        [UnityTest]
        public IEnumerator ShouldRotateDownDHVWithMouseWhenDraggingRightButtonUp()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.7f), 10);

            Assert.Less(dollhouseViewWrapper.localEulerAngles.x, initialRotationX);
        }

        [UnityTest]
        public IEnumerator ShouldRotateLeftDHVWithMouseWhenDraggingRightButtonRight()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.3f, 0.5f), 10);

            float normalizedFinalRotation = NormalizeAngle(dollhouseViewWrapper.localEulerAngles.y);
            Assert.Greater(normalizedFinalRotation, initialRotationY);
        }

        [UnityTest]
        public IEnumerator ShouldRotateRightDHVWithMouseWhenDraggingRightButtonLeft()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.5f), 10);

            float normalizedFinalRotation = NormalizeAngle(dollhouseViewWrapper.localEulerAngles.y);
            Assert.Greater(normalizedFinalRotation, initialRotationY);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMaxVerticalRotationAngleWithMouse()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, -100.0f), 10);

            Assert.AreEqual(89.9f, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }

        [UnityTest]
        public IEnumerator ShouldClampVerticalRotationToMinVerticalRotationAngleWithMouse()
        {
            Assert.AreEqual(initialRotationX, dollhouseViewWrapper.localEulerAngles.x);
            Assert.AreEqual(initialRotationY, dollhouseViewWrapper.localEulerAngles.y);

            yield return PointerUtils.DragMouseRightButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 100.0f), 10);

            Assert.AreEqual(10f, dollhouseViewWrapper.localEulerAngles.x, errorToleranceInDegrees);
        }
    }
}