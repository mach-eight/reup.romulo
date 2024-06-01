using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.helperInterfaces;

namespace Tests.PlayMode.Mocks
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
            Debug.Log("Highlighting object in spy");
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

    }
}