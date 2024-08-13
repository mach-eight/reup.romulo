using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class SelectedObjectsManager : MonoBehaviour, ISelectedObjectsManager, IIsObjectPartOfSelection, IOnAllowSelectionChange
        ,ISelectableObjectsHighlighter
    {
        public event Action<bool> AllowSelectionChanged;
        private IObjectWrapper _objectWrapper;
        public IObjectWrapper objectWrapper { set =>  _objectWrapper = value; }
        private IObjectHighlighter _highlighter;
        public IObjectHighlighter highlighter { set => _highlighter = value; }
        private IMediator _mediator;
        public IMediator mediator { set { _mediator = value; } }
        public IHighlightAnimator highlightAnimator { get; set; }
        public IObjectRegistry objectRegistry { get; set; }

        public ITagsController tagsController { get; set; }

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

        private bool _allowEditSelection = false;
        public bool allowEditSelection { get => _allowEditSelection; set
            {
                _allowEditSelection = value;
                AllowSelectionChanged?.Invoke(_allowEditSelection);
            }
        }

        public GameObject AddObjectToSelection(GameObject selectedObject)
        {
            if (!_allowEditSelection) return null;
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
            if (!_allowEditSelection) return null;
            return ForceRemoveObjectFromSelection(selectedObject);
        }

        public GameObject ForceRemoveObjectFromSelection(GameObject selectedObject)
        {
            wrapperObject = _objectWrapper.UnwrapObject(selectedObject);
            _highlighter.UnhighlightObject(selectedObject);
            return _wrapperObject;
        }

        public bool IsObjectPartOfSelection(GameObject obj)
        {
            List<GameObject> selectedObjects = _objectWrapper.wrappedObjects;
            return GameObjectUtils.IsPartOf(selectedObjects, obj);
        }

        public void HighlightSelectableObjects()
        {
            List<GameObject> selectableObjects = objectRegistry.GetObjects()
                .Where(obj => tagsController.DoesObjectHaveTag(obj, EditionTagsCreator.CreateSelectableTag().id))
                .ToList();
            highlightAnimator.HighlighObjectsEaseInEaseOut(selectableObjects);
        }
    }
}
