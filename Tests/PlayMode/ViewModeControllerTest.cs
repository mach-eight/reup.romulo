using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.controllers;
using NUnit.Framework;
using ReupVirtualTwin.enums;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwinTests.mocks;

namespace ReupVirtualTwinTests.playmode.controller
{
    public class ViewModeControllerTest: MonoBehaviour
    {
        ViewModeController viewModelController;
        CharacterPositionManagerSpy characterPositionManagerSpy;

        [SetUp]
        public void SetUp()
        {
            characterPositionManagerSpy = new CharacterPositionManagerSpy();
            viewModelController = new ViewModeController(characterPositionManagerSpy);
        }

        [Test]
        public void ShouldHaveFPVActivatedByDefault()
        {
            Assert.AreEqual(viewModelController.viewMode, ViewMode.FPV);
        }

        [Test]
        public void ShouldChangeViewModeToHDV()
        {
            viewModelController.ActivateDHV();
            Assert.AreEqual(viewModelController.viewMode, ViewMode.DHV);
        }

        [Test]
        public void ShouldChangeBackViewModeToFPV()
        {
            viewModelController.viewMode = ViewMode.DHV;
            Assert.AreEqual(viewModelController.viewMode, ViewMode.DHV);
            viewModelController.ActivateFPV();
            Assert.AreEqual(viewModelController.viewMode, ViewMode.FPV);
        }

        [Test]
        public void ShouldMakeCharacterKinematic_whenViewModeIsDHV()
        {
            Assert.IsFalse(characterPositionManagerSpy.isKinematic);
            viewModelController.ActivateDHV();
            Assert.IsTrue(characterPositionManagerSpy.isKinematic);
        }
    }
}
