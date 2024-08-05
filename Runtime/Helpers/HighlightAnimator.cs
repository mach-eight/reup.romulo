using ReupVirtualTwin.helperInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class HighlightAnimator : IHighlightAnimator
    {
        HaloApplier haloApplier;

        public HighlightAnimator()
        {
            haloApplier = new HaloApplier();
        }

        public void HighlighObjectsEaseInEaseOut(List<GameObject> objs)
        {
            throw new System.NotImplementedException();
        }
    }
}
