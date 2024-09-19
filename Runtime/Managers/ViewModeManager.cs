using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class ViewModeManager : MonoBehaviour, IViewModeManager
    {
        public ViewMode viewMode = ViewMode.FPV;
        public GameObject dollhouseViewCamera;
        public GameObject character;

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
            character.SetActive(false);
            dollhouseViewCamera.SetActive(true);
        }

        private void ActivateFPVCamera()
        {
            character.SetActive(true);
            dollhouseViewCamera.SetActive(false);
        }
    }
}
