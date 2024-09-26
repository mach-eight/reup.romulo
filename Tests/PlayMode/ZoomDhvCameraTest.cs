using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace ReupVirtualTwin.behaviours
{
    public class ZoomDhvCameraTest
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        InputTestFixture input;
        Mouse mouse;
        Touchscreen touch;
        ZoomDhvCamera zoomDhvCamera;
        CinemachineCamera dhvCamera;
        float initialFieldOfView;
        float minFieldOfView;
        float maxFieldOfView;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            input = sceneObjects.input;
            mouse = InputSystem.AddDevice<Mouse>();
            touch = InputSystem.AddDevice<Touchscreen>();
            zoomDhvCamera = sceneObjects.zoomDhvCamera;
            dhvCamera = sceneObjects.dhvCamera.GetComponent<CinemachineCamera>();
            initialFieldOfView = dhvCamera.Lens.FieldOfView;
            minFieldOfView = zoomDhvCamera.minFieldOfView;
            maxFieldOfView = zoomDhvCamera.maxFieldOfView;
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
        public void ZoomSpeedIsDefined()
        {
            Assert.AreEqual(0.1f, zoomDhvCamera.zoomSpeed);
        }

        [Test]
        public void MinAndMaxFovAreDefined()
        {
            Assert.AreEqual(60, zoomDhvCamera.maxFieldOfView);
            Assert.AreEqual(1, zoomDhvCamera.minFieldOfView);
        }

        [UnityTest]
        public IEnumerator ScrollWheelShouldZoomIn()
        {
            Assert.AreEqual(dhvCamera.Lens.FieldOfView, initialFieldOfView);

            input.Set(mouse.scroll, new Vector2(0, 5));
            yield return null;

            Assert.Less(dhvCamera.Lens.FieldOfView, initialFieldOfView);
        }

        [UnityTest]
        public IEnumerator ScrollWheelShouldZoomOut()
        {
            float newCameraFov = 10f;
            dhvCamera.Lens.FieldOfView = newCameraFov;

            input.Set(mouse.scroll, new Vector2(0, -5));
            yield return null;

            Assert.Greater(dhvCamera.Lens.FieldOfView, newCameraFov);
        }

        [UnityTest]
        public IEnumerator PinchInGestureShouldZoomIn()
        {
            Assert.AreEqual(initialFieldOfView, dhvCamera.Lens.FieldOfView);

            Vector2 startFinger1 = new Vector2(2000, 2000);
            Vector2 startFinger2 = new Vector2(4000, 4000);
            Vector2 endFinger1 = new Vector2(1000, 1000);
            Vector2 endFinger2 = new Vector2(5000, 5000);

            yield return MovePointerUtils.TouchGesture(input, touch, startFinger1, startFinger2, endFinger1, endFinger2, 10);

            Assert.Less(dhvCamera.Lens.FieldOfView, initialFieldOfView);
        }

        [UnityTest]
        public IEnumerator PinchOutGestureShouldZoomOut()
        {
            float newCameraFov = 10f;
            dhvCamera.Lens.FieldOfView = newCameraFov;

            Vector2 startFinger1 = new Vector2(1000, 1000);
            Vector2 startFinger2 = new Vector2(5000, 5000);
            Vector2 endFinger1 = new Vector2(2000, 2000);
            Vector2 endFinger2 = new Vector2(4000, 4000);

            yield return MovePointerUtils.TouchGesture(input, touch, startFinger1, startFinger2, endFinger1, endFinger2, 10);

            Assert.Greater(dhvCamera.Lens.FieldOfView, newCameraFov);
        }

        [UnityTest]
        public IEnumerator ShouldNotExceedMinFov()
        {
            dhvCamera.Lens.FieldOfView = minFieldOfView;

            input.Set(mouse.scroll, new Vector2(0, 10));
            yield return null;

            Assert.GreaterOrEqual(dhvCamera.Lens.FieldOfView, minFieldOfView);
        }

        [UnityTest]
        public IEnumerator ShouldNotExceedMaxFov()
        {
            dhvCamera.Lens.FieldOfView = maxFieldOfView;

            input.Set(mouse.scroll, new Vector2(0, -10));
            yield return null;

            Assert.LessOrEqual(dhvCamera.Lens.FieldOfView, maxFieldOfView);
        }
    }
}