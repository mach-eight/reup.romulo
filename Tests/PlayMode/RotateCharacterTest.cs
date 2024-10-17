using NUnit.Framework;
using ReupVirtualTwin.helpers;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.Video;


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
        float errorThreshold = 1e-5f;
        Camera mainCamera;

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

        [Test]
        public void AngleTest()
        {
            Vector3 v1 = new Vector3(1, 1, 1);
            Vector3 x = new Vector3(1, 0, 0);
            Vector3 y = new Vector3(0, 1, 0);
            Vector3 z = new Vector3(0, 0, 1);
            float angle = Vector3.Angle(v1, x);
            Debug.Log($"53: angle >>>\n{angle}");
            float angle2 = Vector3.Angle(x, v1);
            Debug.Log($"53: angle >>>\n{angle2}");
            float angleY = Vector3.SignedAngle(v1, x, y);
            float angleX = Vector3.SignedAngle(v1, x, x);
            float angleZ = Vector3.SignedAngle(v1, x, z);
            Debug.Log($"60: angleX >>>\n{angleX}");
            Debug.Log($"61: angleY >>>\n{angleY}");
            Debug.Log($"62: angleZ >>>\n{angleZ}");
        }

        [UnityTest]
        public IEnumerator ShouldRotateHorizontallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            float horizontalFov = CameraUtils.GetHorizontalFov(mainCamera);
            float relativeViewPortMovementFromCenter = 0.5f;
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = initialPointerRelativePosition + Vector2.right * relativeViewPortMovementFromCenter;
            int touchId = 0;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, 90);
            // yield return new WaitForSeconds(1);
            float expectedTravelAngleRad = CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, horizontalFov) * Mathf.Rad2Deg;
            Debug.Log($"60: expectedTravelAngleRad >>>\n{expectedTravelAngleRad}");
            Debug.Log($"61: character.eulerAngles.y >>>\n{character.eulerAngles.y}");
            float traveledAngle = MathUtils.NormalizeAngle(character.eulerAngles.y);
            Debug.Log($"62: traveledAngle >>>\n{traveledAngle}");
            // yield return new WaitForSeconds(100);
            Assert.AreEqual(expectedTravelAngleRad, traveledAngle);
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
            yield return PointerUtils.DragMouseLeftButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.49f, 0.5f), 90);
            bool characterTurnedLeft = (character.eulerAngles.y > 180 && character.eulerAngles.y < 360) ||
                (character.eulerAngles.y < 0 && character.eulerAngles.y > -180);
            Assert.IsTrue(characterTurnedLeft);
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
            yield return PointerUtils.DragMouseLeftButton(input, mouse, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.7f), 90);
            bool characterLookedUp = (innerCharacter.eulerAngles.x < 0 && innerCharacter.eulerAngles.x > -180) ||
                (innerCharacter.eulerAngles.x > 180 && innerCharacter.eulerAngles.x < 360);
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            Assert.IsTrue(characterLookedUp);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRotateVerticallyInTouchscreen()
        {
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            AssertUtils.AssertVectorIsZero(innerCharacter.localEulerAngles, errorThreshold);
            int touchId = 0;
            yield return PointerUtils.MoveFinger(input, touch, touchId, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.3f), 90);
            bool characterLookedUp = (innerCharacter.eulerAngles.x < 0 && innerCharacter.eulerAngles.x > -180) ||
                (innerCharacter.eulerAngles.x > 180 && innerCharacter.eulerAngles.x < 360);
            AssertUtils.AssertVectorIsZero(character.eulerAngles, errorThreshold);
            Assert.IsTrue(characterLookedUp);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
            Assert.LessOrEqual(innerCharacter.eulerAngles.y, errorThreshold);
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
