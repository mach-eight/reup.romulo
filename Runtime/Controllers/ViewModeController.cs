using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.enums;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class ViewModeController : IViewModeController
    {
        public ViewMode viewMode = ViewMode.FPV;
        public GameObject firstPersonViewCamera;
        public GameObject dollhouseViewCamera;

        public ViewModeController(GameObject fpvCamera, GameObject dhvCamera)
        {
            firstPersonViewCamera = fpvCamera;
            dollhouseViewCamera = dhvCamera;
        }

        public void ActivateDHV()
        {
            viewMode = ViewMode.DHV;
            ActivateDHVCamera();
        }

        public void ActivateFPV()
        {
            viewMode = ViewMode.FPV;
            ActivateFPVCamera();
        }

        private void ActivateDHVCamera()
        {
            firstPersonViewCamera.SetActive(false);
            dollhouseViewCamera.SetActive(true);
        }

        private void ActivateFPVCamera()
        {
            firstPersonViewCamera.SetActive(true);
            dollhouseViewCamera.SetActive(false);
        }
    }
}
