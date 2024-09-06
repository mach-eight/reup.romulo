using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.helperInterfaces;

namespace ReupVirtualTwinTests.utils.mocks
{
    public class ObjectHighlighterSpy : IObjectHighlighter
    {
        private GameObject highlightedObject;
        private int highlightCount = 0;
        public ObjectHighlighterSpy()
        {
            highlightCount = 0;
        }
        public void HighlightObject(GameObject obj)
        {
            highlightedObject = obj;
            highlightCount++;
        }

        public void UnhighlightObject(GameObject obj)
        {
            highlightedObject = null;
        }

        public GameObject GetHighlightedObject()
        {
            return highlightedObject;
        }

        public int GetHighlightCount()
        {
            return highlightCount;
        }

        public bool HighlightObjectIfNotHighlighted(GameObject obj)
        {
            HighlightObject(obj);
            return true;
        }
    }
}
