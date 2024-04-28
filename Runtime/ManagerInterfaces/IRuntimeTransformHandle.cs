using ReupVirtualTwin.enums;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IRuntimeTransformHandle
    {
        public Transform target { set; }
        public IMediator mediator { set; }
        public bool autoScale { set; }
        public float autoScaleFactor { set; }
        public TransformHandleType type { set; }
    }
}
