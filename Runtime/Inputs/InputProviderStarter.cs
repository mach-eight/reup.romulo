using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.inputs
{
    public class InputProviderStarter : MonoBehaviour
    {
        private InputProvider _inputProvider;

        private void OnEnable()
        {
            _inputProvider = new InputProvider();
            _inputProvider.Enable();
        }
        private void OnDisable()
        {
            _inputProvider.Disable();
        }

    }
}