using ReupVirtualTwin.inputs;
using UnityEngine;
using ReupVirtualTwin.helperInterfaces;
using Zenject;

namespace ReupVirtualTwin.helpers
{
    public class MainCameraToPointerRayProvider : MonoBehaviour, IRayProvider
    {
        private InputProvider inputProvider;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public Ray GetRay()
        {
            return Camera.main.ScreenPointToRay(inputProvider.PointerInput());
        }
    }
}
