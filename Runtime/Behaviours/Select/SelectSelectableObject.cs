using ReupVirtualTwin.managerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{

    [RequireComponent(typeof(ISelectedObjectsManager))]
    public class SelectSelectableObject : SelectObject
    {
        ISelectedObjectsManager _selectedObjectsManager;
        ISelectableObjectsHighlighter _selectableObjectsHighlighter;
        public ISelectableObjectsHighlighter selectableObjectsHighlighter
        {
            get { return _selectableObjectsHighlighter; }
            set { _selectableObjectsHighlighter = value; }
        }
        override protected void Start()
        {
            base.Start();
            _selectedObjectsManager = GetComponent<ISelectedObjectsManager>();
            _selectableObjectsHighlighter = GetComponent<ISelectableObjectsHighlighter>();
        }
        public override void HandleObject(GameObject obj)
        {
            if (_selectedObjectsManager.wrapperDTO.wrappedObjects.Contains(obj))
            {
                _selectedObjectsManager.RemoveObjectFromSelection(obj);
                return;
            }
            _selectedObjectsManager.AddObjectToSelection(obj);
        }
        public override void MissObject()
        {
            if (_selectedObjectsManager.allowEditSelection)
            {
                _selectableObjectsHighlighter.HighlightSelectableObjects();
            }
        }
    }
}
