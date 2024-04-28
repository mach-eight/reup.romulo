using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 21.10.2020
     */
    public class RuntimeTransformHandle : MonoBehaviour, IRuntimeTransformHandle
    {
        public HandleAxes axes = HandleAxes.XYZ;
        public HandleSpace space = HandleSpace.LOCAL;
        private TransformHandleType _type = TransformHandleType.POSITION;
        public TransformHandleType type { set => _type = value; }
        public HandleSnappingType snappingType = HandleSnappingType.RELATIVE;

        public Vector3 positionSnap = Vector3.zero;
        public float rotationSnap = 0;
        public Vector3 scaleSnap = Vector3.zero;

        private bool _autoScale = false;
        public bool autoScale { set => _autoScale = value; }
        private float _autoScaleFactor = 1;
        public float autoScaleFactor { set => _autoScaleFactor = value; }
        public Camera handleCamera;

        private Vector3 _previousMousePosition;
        private HandleBase _previousAxis;
        
        private HandleBase _draggingHandle;

        private TransformHandleType _previousType;
        private HandleAxes _previousAxes;

        private PositionHandle _positionHandle;
        private RotationHandle _rotationHandle;
        private ScaleHandle _scaleHandle;

        private Transform _target;
        public Transform target { set => _target = value; get => _target; }
        private IMediator _mediator;
        public IMediator mediator
        {
            set { _mediator = value; }
        }

        void Start()
        {
            if (handleCamera == null)
                handleCamera = Camera.main;

            _previousType = _type;

            if (_target == null)
                _target = transform;

            CreateHandles();
        }

        void CreateHandles()
        {
            switch (_type)
            {
                case TransformHandleType.POSITION:
                    _positionHandle = gameObject.AddComponent<PositionHandle>().Initialize(this);
                    break;
                case TransformHandleType.ROTATION:
                    _rotationHandle = gameObject.AddComponent<RotationHandle>().Initialize(this);
                    break;
                case TransformHandleType.SCALE:
                    _scaleHandle = gameObject.AddComponent<ScaleHandle>().Initialize(this);
                    break;
            }
            ApplyLayerToChildren(gameObject);
        }

        void Clear()
        {
            _draggingHandle = null;
            
            if (_positionHandle) _positionHandle.Destroy();
            if (_rotationHandle) _rotationHandle.Destroy();
            if (_scaleHandle) _scaleHandle.Destroy();
        }

        void Update()
        {
            if (_autoScale)
                transform.localScale =
                    Vector3.one * (Vector3.Distance(handleCamera.transform.position, transform.position) * _autoScaleFactor) / 15;
            
            if (_previousType != _type || _previousAxes != axes)
            {
                Clear();
                CreateHandles();
                _previousType = _type;
                _previousAxes = axes;
            }

            HandleBase handle = null;
            Vector3 hitPoint = Vector3.zero;
            GetHandle(ref handle, ref hitPoint);

            HandleOverEffect(handle, hitPoint);

            if (Input.GetMouseButton(0) && _draggingHandle != null)
            {
                _draggingHandle.Interact(_previousMousePosition);
            }

            if (Input.GetMouseButtonDown(0) && handle != null)
            {
                _draggingHandle = handle;
                _mediator.Notify(ReupEvent.transformHandleStartItneraction);
                _draggingHandle.StartInteraction(hitPoint);
            }

            if (Input.GetMouseButtonUp(0) && _draggingHandle != null)
            {
                _mediator.Notify(ReupEvent.transformHandleStopInteraction);
                _draggingHandle.EndInteraction();
                _draggingHandle = null;
            }

            _previousMousePosition = Input.mousePosition;

            transform.position = _target.transform.position;
            if (space == HandleSpace.LOCAL || _type == TransformHandleType.SCALE)
            {
                transform.rotation = _target.transform.rotation;
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }

        void HandleOverEffect(HandleBase p_axis, Vector3 p_hitPoint)
        {
            if (_draggingHandle == null && _previousAxis != null && (_previousAxis != p_axis || !_previousAxis.CanInteract(p_hitPoint)))
            {
                _previousAxis.SetDefaultColor();
            }

            if (p_axis != null && _draggingHandle == null && p_axis.CanInteract(p_hitPoint))
            {
                p_axis.SetColor(Color.yellow);
            }

            _previousAxis = p_axis;
        }

        private void GetHandle(ref HandleBase p_handle, ref Vector3 p_hitPoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length == 0)
                return;

            foreach (RaycastHit hit in hits)
            {
                p_handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();

                if (p_handle != null)
                {
                    p_hitPoint = hit.point;
                    return;
                }
            }
        }

        static public RuntimeTransformHandle Create(Transform p_target, TransformHandleType p_handleType)
        {
            RuntimeTransformHandle runtimeTransformHandle = new GameObject().AddComponent<RuntimeTransformHandle>();
            runtimeTransformHandle._target = p_target;
            runtimeTransformHandle._type = p_handleType;

            return runtimeTransformHandle;
        }
        private void ApplyLayerToChildren(GameObject parentGameObj)
        {
            //Debug.Log("appplying layers");
            foreach (Transform transform1 in parentGameObj.transform)
            {
                int layer = parentGameObj.layer;
                transform1.gameObject.layer = layer;
                //Debug.Log($"applying layer to {transform1.name}");
                foreach (Transform transform2 in transform1)
                {
                //Debug.Log($"applying layer to {transform2.name}");
                    transform2.gameObject.layer = layer;
                    foreach (Transform transform3 in transform2)
                    {
                //Debug.Log($"applying layer to {transform3.name}");
                        transform3.gameObject.layer = layer;
                        foreach (Transform transform4 in transform3)
                        {
                //Debug.Log($"applying layer to {transform4.name}");
                            transform4.gameObject.layer = layer;
                            foreach (Transform transform5 in transform4)
                            {
                //Debug.Log($"applying layer to {transform5.name}");
                                transform5.gameObject.layer = layer;
                            }
                        }
                    }
                }
            }
        }
    }
}