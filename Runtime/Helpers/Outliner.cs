using ReupVirtualTwin.helperInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class Outliner : MonoBehaviour, IObjectHighlighter
    {
        public void HighlightObject(GameObject obj)
        {
            Outline outline = obj.AddComponent<Outline>();
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5;
        }

        public void UnhighlightObject(GameObject obj)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null )
            {
                Object.Destroy(outline);
            }
        }
    }
}
