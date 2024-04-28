using ReupVirtualTwin.helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class ShowSlidingPanel : MonoBehaviour
    {
        public GameObject panel;

        private IObjectPool _objectPool;

        void Start()
        {
            _objectPool = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<IObjectPool>();
            _objectPool.PoolObject(panel);
        }
        
        public void ShowPanel()
        {
            _objectPool.GetObjectFromPool(panel.name);
            _objectPool.PoolObject(gameObject);
        }

        public void HidePanel()
        {
            _objectPool.PoolObject(panel);
            _objectPool.GetObjectFromPool(gameObject.name);
        }
    }
}
