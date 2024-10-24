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
        float errorThreshold = 1e-4f;
        float errorAngleThreshold = 1.0f;
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

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            float horizontalFov = CameraUtils.GetHorizontalFov(mainCamera);
            float relativeViewPortMovementFromCenter = 0.2f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.right * relativeViewPortMovementFromCenter;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, steps);
            yield return null;
            float expectedTravelAngleDeg = -CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, horizontalFov) * Mathf.Rad2Deg;
            float traveledAngle = MathUtils.NormalizeAngle(character.eulerAngles.y);
            Assert.AreEqual(expectedTravelAngleDeg, traveledAngle, errorAngleThreshold);
            Assert.LessOrEqual(character.eulerAngles.x, errorThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyWithMouse()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            float horizontalFov = CameraUtils.GetHorizontalFov(mainCamera);
            float relativeViewPortMovementFromCenter = 0.4f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.right * relativeViewPortMovementFromCenter;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, steps);
            yield return null;
            float expectedTravelAngleDeg = -CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, horizontalFov) * Mathf.Rad2Deg;
            float traveledAngle = MathUtils.NormalizeAngle(character.eulerAngles.y);
            Assert.AreEqual(expectedTravelAngleDeg, traveledAngle, errorAngleThreshold);
            Assert.LessOrEqual(character.eulerAngles.x, errorThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyWithMouse()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            float verticalFov = CameraUtils.GetVerticalFov(mainCamera);
            float relativeViewPortMovementFromCenter = 0.4f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.up * relativeViewPortMovementFromCenter;
            yield return PointerUtils.DragMouseLeftButton(input, mouse, initialPointerRelativePosition, finalPointerRelativePosition, steps);
            yield return null;
            float expectedTravelAngleDeg = CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, verticalFov) * Mathf.Rad2Deg;
            float traveledAngle = MathUtils.NormalizeAngle(innerCharacter.eulerAngles.x);
            Assert.AreEqual(expectedTravelAngleDeg, traveledAngle, errorAngleThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            float verticalFov = CameraUtils.GetVerticalFov(mainCamera);
            float relativeViewPortMovementFromCenter = 0.4f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.up * relativeViewPortMovementFromCenter;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, steps);
            yield return null;
            float expectedTravelAngleDeg = CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, verticalFov) * Mathf.Rad2Deg;
            float traveledAngle = MathUtils.NormalizeAngle(innerCharacter.eulerAngles.x);
            Assert.AreEqual(expectedTravelAngleDeg, traveledAngle, errorAngleThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            yield return null;

        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingLeftArrowKey()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            input.Press(keyboard.leftArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.leftArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, errorThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyUsingRightArrowKey()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            input.Press(keyboard.rightArrowKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.rightArrowKey);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsFalse(characterTurnedLeft);
            Assert.LessOrEqual(character.eulerAngles.x, errorThreshold);
            Assert.LessOrEqual(character.eulerAngles.z, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            yield return null;
        }

    }
}
