using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IChangeColorManager
    {
        public void ChangeObjectsColor(List<GameObject> objects, Color color);
    }
}

