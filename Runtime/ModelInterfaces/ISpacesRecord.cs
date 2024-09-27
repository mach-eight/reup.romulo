using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface ISpacesRecord
    {
        public List<ISpaceJumpPoint> jumpPoints { get; set; }
        public void GoToSpace(string spaceJumpPointId, string requestId);
    }

}