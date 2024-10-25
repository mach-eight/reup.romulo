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
        public GameObject houseContainer;

        private void Awake()
        {
            ActivateFPV();
        }

        public void ActivateDHV()
        {
            viewMode = ViewMode.dollhouse;
            character.SetActive(false);
            dollhouseViewWrapper.SetActive(true);
            houseContainer.SetActive(false);
        }

        public void ActivateFPV()
        {
            viewMode = ViewMode.firstPerson;
            character.SetActive(true);
            dollhouseViewWrapper.SetActive(false);
            houseContainer.SetActive(true);
        }

    }
}
