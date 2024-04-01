using ReupVirtualTwin.enums;
using UnityEngine;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.dataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class TransformObjectsManager : MonoBehaviour, ITransformObjectsManager
    {
        private bool _active = false;
        public bool active { get { return _active; } }
        private GameObject _runtimeTransformObj;
        public GameObject runtimeTransformObj { set => _runtimeTransformObj = value; }
        private IRuntimeTransformHandle _runtimeTransformHandle;
        private int _runtimeTransformLayer = 6;
        private IMediator _mediator;
        public IMediator mediator { set { _mediator = value; } }
        private ITagsController _tagsController;
        public ITagsController tagsController { set =>  _tagsController = value; }

        private GameObject _transformWrapper;
        public ObjectWrapperDTO wrapper
        {
            set
            {
                if (value.wrapper == null || !AreWrappedObjectsTransformable(value.wrappedObjects))
                {
                    DeactivateTransformMode();
                    return;
                }
                _transformWrapper = value.wrapper;
                if (_transformWrapper != null)
                {
                    _runtimeTransformHandle.target = _transformWrapper.transform;
                }
            }
        }

        private void Start()
        {
            _runtimeTransformHandle = _runtimeTransformObj.GetComponent<IRuntimeTransformHandle>();
            _runtimeTransformHandle.mediator = _mediator;
            _runtimeTransformHandle.autoScale = true;
            _runtimeTransformHandle.autoScaleFactor = 1.0f;
            _runtimeTransformObj.layer = _runtimeTransformLayer;
            _runtimeTransformObj.SetActive(false);
        }
        public void ActivateTransformMode(ObjectWrapperDTO wrapperDTO, TransformMode mode)
        {
            if (wrapperDTO == null || wrapperDTO.wrapper == null)
            {
                throw new ArgumentException("section wrapper is null, can't activate transform mode");
            }
            if (!AreWrappedObjectsTransformable(wrapperDTO.wrappedObjects))
            {
                return;
            }
            if (mode == TransformMode.PositionMode)
            {
                _runtimeTransformHandle.type = TransformHandleType.POSITION;
            }
            if (mode == TransformMode.RotationMode)
            {
                _runtimeTransformHandle.type = TransformHandleType.ROTATION;
            }
            _active = true;
            this.wrapper = wrapperDTO;
            _runtimeTransformObj.SetActive(true);
            if (mode == TransformMode.PositionMode)
            {
                _mediator.Notify(ReupEvent.positionTransformModeActivated);
            }
            if (mode == TransformMode.RotationMode)
            {
                _mediator.Notify(ReupEvent.rotationTransformModeActivated);
            }
        }

        public void DeactivateTransformMode()
        {
            if (!_active) throw new InvalidOperationException("Can't deactivate transform mode, no active transform mode is active to begin with");
            _active = false;
            _runtimeTransformObj.SetActive(false);
            _mediator.Notify(ReupEvent.transformModeDeactivated);
        }

        private bool AreWrappedObjectsTransformable(List<GameObject> objects)
        {
            if (objects.Count == 0) return false;
            return objects.All(obj => _tagsController.DoesObjectHaveTag(obj, EditionTag.TRANSFORMABLE));
        }
    }
}
