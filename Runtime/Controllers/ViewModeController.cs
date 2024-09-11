using Newtonsoft.Json.Linq;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class ViewModeController : IViewModeController
    {
        public ViewMode viewMode = ViewMode.FPV;
        ICharacterPositionManager characterPositionManager;

        public ViewModeController(ICharacterPositionManager characterPositionManager)
        {
           this.characterPositionManager = characterPositionManager;
        }

        public void ActivateDHV()
        {
            viewMode = ViewMode.DHV;
            characterPositionManager.MakeKinematic();
        }

        public void ActivateFPV()
        {
            viewMode = ViewMode.FPV;
        }
    }
}
