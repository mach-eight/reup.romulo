using UnityEngine;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.managers
{
    public class SelectedObjectsManager : MonoBehaviour, ISelectedObjectsManager
    {
        private IObjectWrapper _objectWrapper;
        public IObjectWrapper objectWrapper { set =>  _objectWrapper = value; }
        private IObjectHighlighter _highlighter;
        public IObjectHighlighter highlighter { set => _highlighter = value; }
        private IMediator _mediator;
        public IMediator mediator { set { _mediator = value; } }
        private GameObject _wrapperObject;
        private GameObject wrapperObject
        {
            set
            {
                if (_wrapperObject != null)
                {
                    _highlighter.UnhighlightObject(_wrapperObject);
                }
                Destroy(_wrapperObject);
                _wrapperObject = value;
                if (_wrapperObject != null)
                {
                    _highlighter.HighlightObject(_wrapperObject);
                }
                _mediator.Notify(ReupEvent.setSelectedObjects, wrapperDTO);
            }
        }
        public ObjectWrapperDTO wrapperDTO
        {
            get
            {
                return new ObjectWrapperDTO
                {
                    wrapper = _wrapperObject,
                    wrappedObjects = _objectWrapper.wrappedObjects,
                };
            }
        }

        private bool _allowSelection = false;
        public bool allowSelection { get => _allowSelection; set
            {
                _allowSelection = value;
                if (!_allowSelection)
                {
                    ClearSelection();
                }
            }
        }


        public GameObject AddObjectToSelection(GameObject selectedObject)
        {
            if (!_allowSelection) return null;
            wrapperObject = _objectWrapper.WrapObject(selectedObject);
            return _wrapperObject;
        }

        public void ClearSelection()
        {
            if (_objectWrapper.wrappedObjects.Count > 0)
            {
                _objectWrapper.DeWrapAll();
            }
            if (_wrapperObject != null)
            {
                wrapperObject = null;
            }
        }

        public GameObject RemoveObjectFromSelection(GameObject selectedObject)
        {
            wrapperObject = _objectWrapper.UnwrapObject(selectedObject);
            _highlighter.UnhighlightObject(selectedObject);
            return _wrapperObject;
        }
    }
}
