using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface IHighlightAnimator
    {
        public void HighlighObjectsEaseInEaseOut(List<GameObject> objs);
    }
}
