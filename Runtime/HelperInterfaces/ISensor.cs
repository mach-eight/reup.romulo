using UnityEngine;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface ISensor
    {
        public RaycastHit? Sense();
    }
}
