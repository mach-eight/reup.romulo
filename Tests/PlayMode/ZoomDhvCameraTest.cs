using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.behaviours
{
    public class ZoomDhvCameraTest
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        InputTestFixture input;
        Mouse mouse;
        Touchscreen touch;
        ZoomDhvCamera zoomDhvBehavior;
        CinemachineCamera dhvCineMachineComponent;
        float initialFieldOfView;
        float minFieldOfView;
        float maxFieldOfView;
        float scrollStep = 120;
        float errorToleranceInDegrees = 0.1f;
        int zoomSepts = 10;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            input = sceneObjects.input;
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            zoomDhvBehavior = sceneObjects.zoomDhvCameraBehavior;
            dhvCineMachineComponent = sceneObjects.dhvCamera.GetComponent<CinemachineCamera>();
            initialFieldOfView = dhvCineMachineComponent.Lens.FieldOfView;
            minFieldOfView = zoomDhvBehavior.minFieldOfView;
            maxFieldOfView = zoomDhvBehavior.maxFieldOfView;
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
        public void ScrollZoomSpeedIsDefined()
        {
            Assert.AreEqual(0.1f, zoomDhvBehavior.scrollZoomSpeedMultiplier);
        }

        [Test]
        public void ScrollZoomScaleFactorIsDefined()
        {
            Assert.AreEqual(0.975f, zoomDhvBehavior.scrollZoomScaleFactor);
        }

        [Test]
        public void MinAndMaxFovAreDefined()
        {
            Assert.AreEqual(60, zoomDhvBehavior.maxFieldOfView);
            Assert.AreEqual(1, zoomDhvBehavior.minFieldOfView);
        }

        [UnityTest]
        public IEnumerator ScrollWheelShouldZoomInAndOut()
        {
            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, initialFieldOfView);

            Vector2 zoomInScrollInput = new Vector2(0, scrollStep);
            float expectedZoomInFov = CalculateNextScrollZoomFov(zoomInScrollInput);
            yield return SimulateScrollInput(zoomInScrollInput, 1);
            yield return WaitForZoomToFinalize();
            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, expectedZoomInFov, errorToleranceInDegrees);

            Vector2 zoomOutScrollInput = new Vector2(0, -scrollStep);
            float expectedZoomOutFov = CalculateNextScrollZoomFov(zoomOutScrollInput);
            yield return SimulateScrollInput(zoomOutScrollInput, 1);
            yield return WaitForZoomToFinalize();
            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, expectedZoomOutFov, errorToleranceInDegrees);

            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, initialFieldOfView, errorToleranceInDegrees);
            Assert.Greater(expectedZoomOutFov, expectedZoomInFov);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PinchInGestureShouldZoomInAndOut()
        {
            Assert.AreEqual(initialFieldOfView, dhvCineMachineComponent.Lens.FieldOfView);
            Vector2 ZoomInStartFinger1 = new Vector2(200, 200);
            Vector2 ZoomInStartFinger2 = new Vector2(400, 400);
            Vector2 ZoomInEndFinger1 = new Vector2(100, 100);
            Vector2 ZoomInEndFinger2 = new Vector2(500, 500);

            float expectedZoomInFov = calculateNextPinchZoomFov(ZoomInStartFinger1, ZoomInStartFinger2, ZoomInEndFinger1, ZoomInEndFinger2);  
            yield return MovePointerUtils.TouchGesture(input, touch, ZoomInStartFinger1, ZoomInStartFinger2, ZoomInEndFinger1, ZoomInEndFinger2, zoomSepts);
            yield return WaitForZoomToFinalize();
            Assert.AreEqual(expectedZoomInFov, dhvCineMachineComponent.Lens.FieldOfView, errorToleranceInDegrees);

            Vector2 ZoomOutStartFinger1 = new Vector2(100, 100);
            Vector2 ZoomOutStartFinger2 = new Vector2(500, 500);
            Vector2 ZoomOutEndFinger1 = new Vector2(200, 200);
            Vector2 ZoomOutEndFinger2 = new Vector2(400, 400);

            float expectedZoomOutFov = calculateNextPinchZoomFov(ZoomOutStartFinger1, ZoomOutStartFinger2, ZoomOutEndFinger1, ZoomOutEndFinger2);  
            yield return MovePointerUtils.TouchGesture(input, touch, ZoomOutStartFinger1, ZoomOutStartFinger2, ZoomOutEndFinger1, ZoomOutEndFinger2, zoomSepts);
            yield return WaitForZoomToFinalize();
            Assert.AreEqual(expectedZoomOutFov, dhvCineMachineComponent.Lens.FieldOfView, errorToleranceInDegrees);

            Assert.AreEqual(initialFieldOfView, dhvCineMachineComponent.Lens.FieldOfView, errorToleranceInDegrees);
            Assert.Greater(expectedZoomOutFov, expectedZoomInFov);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotExceedMinFov()
        {
            Assert.AreEqual(initialFieldOfView, dhvCineMachineComponent.Lens.FieldOfView);

            Vector2 scrollInput = new Vector2(0, scrollStep);

            yield return SimulateScrollInput(scrollInput, 100);
            yield return WaitForZoomToFinalize();

            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, minFieldOfView, errorToleranceInDegrees);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotExceedMaxFov()
        {
            Assert.AreEqual(initialFieldOfView, dhvCineMachineComponent.Lens.FieldOfView);

            Vector2 scrollInput = new Vector2(0, -scrollStep );

            yield return SimulateScrollInput(scrollInput, 100);
            yield return WaitForZoomToFinalize();

            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, maxFieldOfView, errorToleranceInDegrees);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldClampZoomInput_WhenUsingGranularTrackpadInput()
        {
            Assert.AreEqual(initialFieldOfView, dhvCineMachineComponent.Lens.FieldOfView);

            Vector2 scrollInput = new Vector2(0, 300);
            Vector2 expectedClampedInput = new Vector2(0, scrollStep);
            float expectedFov = CalculateNextScrollZoomFov(expectedClampedInput);

            yield return SimulateScrollInput(scrollInput, 1);
            yield return WaitForZoomToFinalize();

            Assert.AreEqual(dhvCineMachineComponent.Lens.FieldOfView, expectedFov, errorToleranceInDegrees);
            yield return null;
        }

        private float CalculateNextScrollZoomFov(Vector2 scrollInput)
        {
            float clapZoomInput = Mathf.Clamp(scrollInput.y, -scrollStep, scrollStep);
            float zoomAmount = clapZoomInput * zoomDhvBehavior.scrollZoomSpeedMultiplier;
            float exponentialZoom = dhvCineMachineComponent.Lens.FieldOfView * Mathf.Pow(zoomDhvBehavior.scrollZoomScaleFactor, zoomAmount);
            return Mathf.Clamp(exponentialZoom, minFieldOfView, maxFieldOfView);
        }

        private float calculateNextPinchZoomFov(Vector2 startFinger1, Vector2 startFinger2, Vector2 endFinger1, Vector2 endFinger2)
        {
            float initialPinchDistance = Vector2.Distance(startFinger1, startFinger2);
            float currentDistance = Vector2.Distance(endFinger1, endFinger2);
            float zoomFactor = currentDistance / initialPinchDistance;
            return Mathf.Clamp(dhvCineMachineComponent.Lens.FieldOfView / zoomFactor, minFieldOfView, maxFieldOfView);
        }

        private IEnumerator WaitForZoomToFinalize()
        {
            yield return new WaitForSeconds(zoomDhvBehavior.smoothTime * zoomSepts);
        }

        private IEnumerator SimulateScrollInput(Vector2 scrollInput, int scrollRepetitions)
        {
            for (int i = 0; i < scrollRepetitions; i++)
            {
                input.Set(mouse.scroll, scrollInput);
                yield return null;
            }
        }

    }
}