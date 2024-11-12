using NUnit.Framework;
using ReupVirtualTwin.helpers;
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
        float errorAngleThreshold = 0.1f;
        Camera mainCamera;
        int steps = 10;
        int touchId = 0;

        float timeInSecsForHoldingButton = 0.25f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            mainCamera = sceneObjects.mainCamera;
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

        void AssertExpectedAngle(float expectedAngle, float actualAngle)
        {
            Assert.LessOrEqual(Mathf.Abs(MathUtils.NormalizeAngle(expectedAngle) - MathUtils.NormalizeAngle(actualAngle)), errorAngleThreshold);
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            float relativeViewPortMovementFromCenter = 0.2f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Ray initialRay = mainCamera.ViewportPointToRay(initialPointerRelativePosition);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.right * relativeViewPortMovementFromCenter;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, steps, false);
            yield return YieldUtils.WaitForFrames(5);
            Ray endRay = mainCamera.ViewportPointToRay(finalPointerRelativePosition);
            float angleDifference = Vector3.Angle(initialRay.direction, endRay.direction);
            AssertExpectedAngle(0, angleDifference);
            AssertExpectedAngle(0, character.eulerAngles.x);
            AssertExpectedAngle(0, character.eulerAngles.z);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyWithMouse()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            float relativeViewPortMovementFromCenter = 0.4f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Ray initialRay = mainCamera.ViewportPointToRay(initialPointerRelativePosition);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.right * relativeViewPortMovementFromCenter;
            yield return PointerUtils.DragMouseLeftButton(input, mouse, initialPointerRelativePosition, finalPointerRelativePosition, steps, false);
            yield return YieldUtils.WaitForFrames(5);
            Ray endRay = mainCamera.ViewportPointToRay(finalPointerRelativePosition);
            float angleDifference = Vector3.Angle(initialRay.direction, endRay.direction);
            AssertExpectedAngle(0, angleDifference);
            AssertExpectedAngle(0, character.eulerAngles.x);
            AssertExpectedAngle(0, character.eulerAngles.z);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyWithMouse()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            float relativeViewPortMovementFromCenter = 0.4f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Ray initialRay = mainCamera.ViewportPointToRay(initialPointerRelativePosition);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.up * relativeViewPortMovementFromCenter;
            yield return PointerUtils.DragMouseLeftButton(input, mouse, initialPointerRelativePosition, finalPointerRelativePosition, steps, false);
            yield return YieldUtils.WaitForFrames(5);
            Ray endRay = mainCamera.ViewportPointToRay(finalPointerRelativePosition);
            float angleDifference = Vector3.Angle(initialRay.direction, endRay.direction);
            AssertExpectedAngle(0, angleDifference);
            AssertExpectedAngle(0, innerCharacter.eulerAngles.z);
            AssertExpectedAngle(0, innerCharacter.eulerAngles.y);
            AssertExpectedAngle(0, character.eulerAngles.x);
            AssertExpectedAngle(0, character.eulerAngles.y);
            AssertExpectedAngle(0, character.eulerAngles.z);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            float relativeViewPortMovementFromCenter = 0.4f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Ray initialRay = mainCamera.ViewportPointToRay(initialPointerRelativePosition);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.up * relativeViewPortMovementFromCenter;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, steps, false);
            yield return YieldUtils.WaitForFrames(6);
            Ray endRay = mainCamera.ViewportPointToRay(finalPointerRelativePosition);
            float angleDifference = Vector3.Angle(initialRay.direction, endRay.direction);
            AssertExpectedAngle(0, angleDifference);
            AssertExpectedAngle(0, innerCharacter.eulerAngles.z);
            AssertExpectedAngle(0, innerCharacter.eulerAngles.y);
            AssertExpectedAngle(0, character.eulerAngles.x);
            AssertExpectedAngle(0, character.eulerAngles.y);
            AssertExpectedAngle(0, character.eulerAngles.z);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingLeftArrowKey()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            input.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.leftArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, errorAngleThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingRightArrowKey()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            input.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.rightArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsFalse(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, errorAngleThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, errorAngleThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorAngleThreshold);
            yield return null;
        }

    }
}
