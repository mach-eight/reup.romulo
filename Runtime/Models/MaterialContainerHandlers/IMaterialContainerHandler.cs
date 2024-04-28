using UnityEngine;

namespace ReupVirtualTwin.models
{
    public interface IMaterialContainerHandler : IMaterialsContainerHider
    {
        public GameObject CreateContainer(MaterialSelectionTrigger trigger);
        public void SetNewMaterial(Material material);
    }
}
