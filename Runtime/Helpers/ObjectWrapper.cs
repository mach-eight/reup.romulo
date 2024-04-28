using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.dataModels;
using System.Linq;

namespace ReupVirtualTwin.helpers
{
    public class ObjectWrapper : IObjectWrapper
    {
        List<WrappedObjectInfo> _wrappedObjectsInfo = new List<WrappedObjectInfo>();
        GameObject objectWrapper;

        public GameObject wrapper {  get { return objectWrapper; } }

        public List<GameObject> wrappedObjects
        {
            get
            {
                return _wrappedObjectsInfo.Select(obj => obj.wrappedObject).ToList();
            }
        }

        private class WrappedObjectInfo
        {
            public Transform originalParent;
            public GameObject wrappedObject;
        }

        public GameObject WrapObject(GameObject obj)
        {
            _wrappedObjectsInfo.Add(new WrappedObjectInfo
            {
                originalParent = obj.transform.parent,
                wrappedObject = obj
            });
            UpdateWrapper();
            return objectWrapper;
        }

        public GameObject UnwrapObject(GameObject obj)
        {
            WrappedObjectInfo objectInfo = _wrappedObjectsInfo.FirstOrDefault(x => x.wrappedObject == obj);

            ReturnObjectToParent(objectInfo);
            _wrappedObjectsInfo.Remove(objectInfo);
            UpdateWrapper();
            return objectWrapper;
        }
        void UpdateWrapper()
        {
            if (_wrappedObjectsInfo.Count == 0)
            {
                DestroyOldWrapper();
                return;
            }
            Vector3 selectionCenter = GetSelectionCenter();
            GameObject newWrapper = CreateCenteredWrapper(selectionCenter);
            foreach(WrappedObjectInfo obj in _wrappedObjectsInfo)
            {
                obj.wrappedObject.transform.SetParent(newWrapper.transform, true);
            }
            DestroyOldWrapper();
            objectWrapper = newWrapper;
        }
        void DestroyOldWrapper()
        {
            if (objectWrapper != null)
            {
                GameObject.Destroy(objectWrapper);
                objectWrapper = null;
            }
        }

        Vector3 GetSelectionCenter()
        {
            List<Vector3> centers = new List<Vector3>();
            foreach(WrappedObjectInfo obj in _wrappedObjectsInfo)
            {
                centers.Add(GetObjectCenter(obj.wrappedObject));
            }
            return VectorUtils.meanVector3(centers);
        }

        Vector3 GetObjectCenter(GameObject obj)
        {
            ObjectBorder? objectBorder = ReupMeshUtils.GetObjectTreeBorder(obj);
            if (objectBorder == null)
            {
                throw new System.Exception($"No mesh for selected object {obj.name}");
            }
            Vector3 meshCenter = ((ObjectBorder)objectBorder).TransformToCenterSize().center;
            return meshCenter;
        }

        GameObject CreateCenteredWrapper(Vector3 center)
        {
            GameObject wrapper = new GameObject("SelectionWrapper");
            wrapper.transform.position = center;
            return wrapper;
        }

        public GameObject WrapObjects(GameObject[] objs)
        {
            foreach(GameObject obj in objs)
            {
                WrapObject(obj);
            }
            return objectWrapper;
        }

        public void DeWrapAll()
        {
            foreach(WrappedObjectInfo obj in _wrappedObjectsInfo)
            {
                ReturnObjectToParent(obj);
            }
            _wrappedObjectsInfo.Clear();
        }
        void ReturnObjectToParent(WrappedObjectInfo objectInfo)
        {
            objectInfo.wrappedObject.transform.SetParent(objectInfo.originalParent, true);
        }
    }
}
