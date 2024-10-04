using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface ISpacesRecord
    {
        public List<ISpaceJumpPoint> jumpPoints { get; set; }
        public void GoToSpace(JObject jumpInfo);
    }

}