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
        public GameObject character;

        public ViewModeController(GameObject character, GameObject fpvCamera, GameObject dhvCamera)
        {
            this.character = character;
            firstPersonViewCamera = fpvCamera;
            dollhouseViewCamera = dhvCamera;
        }

        public void ActivateDHV()
        {
            viewMode = ViewMode.DHV;
            ActivateDHVCamera();
            character.SetActive(false);
        }

        public void ActivateFPV()
        {
            viewMode = ViewMode.FPV;
            ActivateFPVCamera();
            character.SetActive(true);
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
