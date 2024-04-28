using UnityEngine;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface IObjectHighlighter
    {
        public void HighlightObject(GameObject obj);
        public void UnhighlightObject(GameObject obj);
    }
}
