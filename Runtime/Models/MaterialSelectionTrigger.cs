using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.models
{
    public class MaterialSelectionTrigger : MonoBehaviour, IMaterialSelectionTrigger
    {
        public List<Material> selectableMaterials;
        public List<GameObject> objects;
        //[HideInInspector]
        public int[] submeshIndexes;


        IMaterialContainerHandler _containerHandler;

        private void Start()
        {
            _containerHandler = ObjectFinder.FindExtensionsTriggers().GetComponent<IMaterialContainerHandler>();
        }

        public GameObject CreateContainer()
        {
            return _containerHandler.CreateContainer(this);
        }
    }
}
