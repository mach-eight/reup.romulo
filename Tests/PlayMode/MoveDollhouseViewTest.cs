using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEditor;
using ReupVirtualTwin.helpers;


namespace ReupVirtualTwinTests.behaviours
{
    public class MoveDollhouseViewTest : MonoBehaviour
    {
        GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Cube.prefab");
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform dollhouseViewWrapper;
        Keyboard keyboard;
        Mouse mouse;
        Touchscreen touch;
        float moveSpeedMetresPerSecond;
        float limitFromBuildingInMeters;
        float timeInSecsForHoldingButton = 0.25f;
        float errorToleranceInMeters = 0.01f;
        int pointerSteps = 1000;
        Camera mainCamera;
        GameObject cube;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(cubePrefab);
            cube = sceneObjects.building;
            input = sceneObjects.input;
            mainCamera = sceneObjects.mainCamera;
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            limitFromBuildingInMeters = sceneObjects.zoomPositionRotationDHVController.limitDistanceFromBuildingInMeters;
            dollhouseViewWrapper = sceneObjects.dollhouseViewWrapper;
            sceneObjects.viewModeManager.ActivateDHV();
            yield return null;
            GetCameraInfoAfterTurningOnDHV();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        void GetCameraInfoAfterTurningOnDHV()
        {
            moveSpeedMetresPerSecond = sceneObjects.zoomPositionRotationDHVController.keyboardMoveCameraRelativeSpeed;
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
            yield return PointerUtils.MoveMouse(input, mouse, Vector2.zero, new Vector2(1, 1), pointerSteps);
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            yield return null;
        }

        Vector3 GetHitPointWhenSelectingCubePrefabFromDHVCamera(Vector3 DHVCameraPosition)
        {
            // This is a simplified estimation assuming the cube is located at (0,0,0) and it has a scale of (1,1,1)
            Assert.AreEqual(Vector3.zero, cube.transform.position);
            Assert.AreEqual(Vector3.one, cube.transform.localScale);
            // And also it's assuming the camera x position is 0, it's elevation angle from the origin is less than 45 degrees
            // and it's looking at the cube from the -z axis
            Assert.Zero(DHVCameraPosition.x);
            Assert.Less(DHVCameraPosition.z, 0);
            Assert.Less(Mathf.Atan(DHVCameraPosition.y / -DHVCameraPosition.z) * Mathf.Rad2Deg, 45);
            float cubeHalfSize = 0.5f;
            float cameraAngle = Mathf.Atan(DHVCameraPosition.y / DHVCameraPosition.z);
            float hitPointHeight = cubeHalfSize * -Mathf.Tan(cameraAngle);
            return new Vector3(0, hitPointHeight, -cubeHalfSize);
        }

        Vector3 GetExpectedCameraPositionAfterSidewayMovement(float relativeViewPortMovementFromCenter)
        {
            float horizontalFov = CameraUtils.GetHorizontalFov(mainCamera);
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 expectedCubeHitRayPosition = GetHitPointWhenSelectingCubePrefabFromDHVCamera(cameraPosition);
            float distanceFromCameraToHitPoint = Vector3.Distance(cameraPosition, expectedCubeHitRayPosition);
            float travelAngleRad = CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, horizontalFov);
            float expectedCameraDistanceMovement = distanceFromCameraToHitPoint * Mathf.Tan(travelAngleRad);
            Vector3 expectedCameraPosition = mainCamera.transform.position - new Vector3(expectedCameraDistanceMovement, 0, 0);
            return expectedCameraPosition;
        }

        Vector3 GetExpectedCameraPositionAfterForwardMovement(float relativeViewPortMovementFromCenter)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 expectedCubeHitRayPosition = GetHitPointWhenSelectingCubePrefabFromDHVCamera(cameraPosition);
            float verticalFov = CameraUtils.GetVerticalFov(mainCamera);
            float travelAngleRad = CameraUtils.GetTravelAngleFromViewPortCenterInRad(relativeViewPortMovementFromCenter, verticalFov);
            Vector3 cameraPositionRelativeToHitPoint = cameraPosition - expectedCubeHitRayPosition;
            float cameraHeight = cameraPositionRelativeToHitPoint.y;
            float distanceFromCameraToHitPoint = Vector3.Distance(cameraPosition, expectedCubeHitRayPosition);
            float cameraAttackAngleRad = Mathf.Asin(cameraHeight / distanceFromCameraToHitPoint);
            float expectedCameraDistanceMovement = cameraHeight * ((1 / Mathf.Tan(cameraAttackAngleRad)) - (1 / Mathf.Tan(cameraAttackAngleRad + travelAngleRad)));
            Vector3 expectedCameraPosition = cameraPosition + new Vector3(0, 0, expectedCameraDistanceMovement);
            return expectedCameraPosition;
        }

        [UnityTest]
        public IEnumerator ShouldMoveSidewaysWithMouse()
        {
            float relativeToViewPortPointerMovement = 0.1f;
            Vector3 expectedCameraPosition = GetExpectedCameraPositionAfterSidewayMovement(relativeToViewPortPointerMovement);
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = new Vector2(initialPointerRelativePosition.x + relativeToViewPortPointerMovement, initialPointerRelativePosition.y);
            yield return PointerUtils.DragMouseLeftButton(input, mouse, initialPointerRelativePosition, finalPointerRelativePosition, pointerSteps);
            float differenceFromExpected = Vector3.Distance(expectedCameraPosition, mainCamera.transform.position);
            Assert.LessOrEqual(differenceFromExpected, errorToleranceInMeters);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveForwardWithMouse()
        {
            float relativeToViewPortPointerMovement = 0.1f;
            Vector3 expectedCameraPosition = GetExpectedCameraPositionAfterForwardMovement(relativeToViewPortPointerMovement);
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = new Vector2(initialPointerRelativePosition.x, initialPointerRelativePosition.y - relativeToViewPortPointerMovement);
            yield return PointerUtils.DragMouseLeftButton(input, mouse, initialPointerRelativePosition, finalPointerRelativePosition, pointerSteps);
            float differenceFromExpected = Vector3.Distance(expectedCameraPosition, mainCamera.transform.position);
            Assert.LessOrEqual(differenceFromExpected, errorToleranceInMeters);
            yield return null;
        }


        [UnityTest]
        public IEnumerator ShouldMoveSidewaysWithTouch()
        {
            float relativeToViewPortPointerMovement = 0.1f;
            Vector3 expectedCameraPosition = GetExpectedCameraPositionAfterSidewayMovement(relativeToViewPortPointerMovement);
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = new Vector2(initialPointerRelativePosition.x + relativeToViewPortPointerMovement, initialPointerRelativePosition.y);
            int touchId = 0;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, pointerSteps);
            float differenceFromExpected = Vector3.Distance(expectedCameraPosition, mainCamera.transform.position);
            Assert.LessOrEqual(differenceFromExpected, errorToleranceInMeters);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveForwardWithTouch()
        {
            float relativeToViewPortPointerMovement = 0.1f;
            Vector3 expectedCameraPosition = GetExpectedCameraPositionAfterForwardMovement(relativeToViewPortPointerMovement);
            Vector2 initialPointerRelativePosition = new Vector2(0.5f, 0.5f);
            Vector2 finalPointerRelativePosition = new Vector2(initialPointerRelativePosition.x, initialPointerRelativePosition.y - relativeToViewPortPointerMovement);
            int touchId = 0;
            yield return PointerUtils.MoveFinger(input, touch, touchId, initialPointerRelativePosition, finalPointerRelativePosition, pointerSteps);
            float differenceFromExpected = Vector3.Distance(expectedCameraPosition, mainCamera.transform.position);
            Assert.LessOrEqual(differenceFromExpected, errorToleranceInMeters);
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
            yield return PointerUtils.AbsolutePositionTouchGesture(input, touch, startFinger1, startFinger2, endFinger1, endFinger2, pointerSteps);

            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotMoveWhenDraggingAndThenZooming()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            input.BeginTouch(0, new Vector2(200, 200), true, touch);
            input.BeginTouch(1, new Vector2(400, 400), true, touch);
            yield return null;
            input.MoveTouch(0, new Vector2(100, 100));
            yield return null;
            input.EndTouch(1, new Vector2(400, 400), Vector2.zero, true, touch);
            yield return null;
            Vector3 finalPosition = dollhouseViewWrapper.position;
            Assert.AreEqual(0, finalPosition.x, errorToleranceInMeters);
            Assert.AreEqual(0, finalPosition.y, errorToleranceInMeters);
            Assert.AreEqual(0, finalPosition.z, errorToleranceInMeters);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveCameraMovingTwoTouchPoints()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            float relativeToViewPortPointerMovement = 0.1f;
            Vector3 expectedCameraPosition = GetExpectedCameraPositionAfterForwardMovement(relativeToViewPortPointerMovement);
            float initialFinger0Y = 0.4f;
            Vector2[] startFingerPositions = new[] {
                new Vector2(0.5f, initialFinger0Y), new Vector2(0.5f, 1.0f - initialFinger0Y) // the mean of the two fingers is 0.5, 0.5
            };
            Vector2[] EndFingerPositions = new[] {
                new Vector2(0.5f, initialFinger0Y), // the finger 0 is not moving
                new Vector2(0.5f, 1.0f - initialFinger0Y - 2 * relativeToViewPortPointerMovement), // the end mean of the two fingers is relativeToViewPortPointerMovement below the initial mean
            };
            yield return PointerUtils.TouchGesture(input, touch, startFingerPositions[0], startFingerPositions[1], EndFingerPositions[0], EndFingerPositions[1], pointerSteps);
            yield return new WaitForSeconds(50);
            float differenceFromExpected = Vector3.Distance(expectedCameraPosition, mainCamera.transform.position);
            Assert.LessOrEqual(differenceFromExpected, errorToleranceInMeters);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldMoveCamera_firstWith1Touch_and_then_with2()
        {
            Assert.AreEqual(Vector3.zero, dollhouseViewWrapper.position);
            float relativeToViewPortPointerMovement = 0.1f;
            Vector3 expectedCameraPosition = GetExpectedCameraPositionAfterForwardMovement(relativeToViewPortPointerMovement);
            int[] fingerIds = new int[] { 0, 1 };
            float initialFinger0Y = 0.6f;
            Vector2[] startFingerPositions = new[] {
                new Vector2(0.5f, initialFinger0Y), new Vector2(0.5f, 1.0f - initialFinger0Y) // the mean of the two fingers is 0.5, 0.5
            };
            Vector2 midFinger0Position = new Vector2(0.5f, 0.5f);
            Vector2 endFinger1Position = new Vector2(0.5f, startFingerPositions[1].y - 2 * relativeToViewPortPointerMovement); // the end mean of the two fingers is relativeToViewPortPointerMovement below the initial mean
            yield return PointerUtils.MoveFinger(input, touch, fingerIds[0], startFingerPositions[0], midFinger0Position, pointerSteps, false);
            Assert.AreNotEqual(Vector3.zero, dollhouseViewWrapper.position);
            yield return PointerUtils.MoveFinger(input, touch, fingerIds[0], midFinger0Position, startFingerPositions[0], pointerSteps, false, false);
            AssertUtils.AssertVectorIsZero(dollhouseViewWrapper.position, errorToleranceInMeters); // the camera returned to it's initial position
            yield return PointerUtils.MoveFinger(input, touch, fingerIds[1], startFingerPositions[1], endFinger1Position, pointerSteps);
            float differenceFromExpected = Vector3.Distance(expectedCameraPosition, mainCamera.transform.position);
            Assert.LessOrEqual(differenceFromExpected, errorToleranceInMeters);
            yield return null;
        }

        [UnityTest]
        public IEnumerator MyTest()
        {
            input.BeginTouch(0, new Vector2(200, 200), true, touch);
            yield return null;
            yield return null;
            yield return null;
            input.BeginTouch(1, new Vector2(200, 200), true, touch);
            yield return null;
            yield return null;
            yield return null;
        }


    }
}
