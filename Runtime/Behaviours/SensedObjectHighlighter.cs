using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class SensedObjectHighlighter : MonoBehaviour
    {
        [HideInInspector]
        public bool enableHighlighting { set => _enableHighlighting = value; get => _enableHighlighting; }
        private bool _enableHighlighting;
        public IObjectSensor objectSensor { set => _objectSensor = value; get => _objectSensor; }
        IObjectSensor _objectSensor;

        public IObjectHighlighter objectHighlighter { set => _objectHighlighter = value; get => _objectHighlighter; }
        IObjectHighlighter _objectHighlighter;

        public IIsObjectPartOfSelection selectedObjectsManager
        {
            set => _selectedObjectsManager = value;
            get => _selectedObjectsManager;
        }
        IIsObjectPartOfSelection _selectedObjectsManager;

        private GameObject highlightedObject;

        private void Update()
        {
            if (!enableHighlighting)
            {
                if (highlightedObject != null)
                {
                    _objectHighlighter.UnhighlightObject(highlightedObject);
                    highlightedObject = null;
                }
                return;
            }
            GameObject sensedObject = _objectSensor.Sense();
            if (sensedObject == highlightedObject && _selectedObjectsManager.IsObjectPartOfSelection(sensedObject))
            {
                objectHighlighter.UnhighlightObject(sensedObject);
            }
            if (sensedObject != highlightedObject)
            {
                if (highlightedObject != null)
                {
                    _objectHighlighter.UnhighlightObject(highlightedObject);
                    highlightedObject = null;
                }
                if (sensedObject != null && !_selectedObjectsManager.IsObjectPartOfSelection(sensedObject))
                {
                    _objectHighlighter.HighlightObject(sensedObject);
                    highlightedObject = sensedObject;
                }
            }
        }

    }
}
