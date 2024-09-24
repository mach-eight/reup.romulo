using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class ViewModeManager : MonoBehaviour, IViewModeManager
    {
        public ViewMode viewMode;
        public GameObject dollhouseViewWrapper;
        public GameObject character;

        private void Awake()
        {
            viewMode = ViewMode.FPV;
            dollhouseViewWrapper.SetActive(false);
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
            character.SetActive(false);
            dollhouseViewWrapper.SetActive(true);
        }

        private void ActivateFPVCamera()
        {
            character.SetActive(true);
            dollhouseViewWrapper.SetActive(false);
        }
    }
}
