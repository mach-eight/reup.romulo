using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

using ReupVirtualTwin.controller;
using NUnit.Framework;
using ReupVirtualTwin.enums;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.playmode.controller
{
    public class ViewModeControllerTest: MonoBehaviour
    {
        ViewModeController viewModelController;

        [SetUp]
        public void SetUp()
        {
            viewModelController = new ViewModeController();
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
    }
}
