using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface IObjectInfo
    {
        Material originalMaterial { get; }
        bool materialWasRestored { get; set; }
        bool materialWasChanged { get; set; }
    }
}
