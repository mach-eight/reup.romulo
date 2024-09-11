using Newtonsoft.Json.Linq;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.enums;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class ViewModeController : IViewModeController
    {
        public ViewMode viewMode = ViewMode.FPV;

        public void ActivateDHV()
        {
            viewMode = ViewMode.DHV;
        }

        public void ActivateFPV()
        {
            throw new System.NotImplementedException();
        }
    }
}
