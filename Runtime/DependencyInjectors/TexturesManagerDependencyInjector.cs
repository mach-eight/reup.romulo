using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.managers;
using UnityEngine;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class TexturesManagerDependencyInjector : MonoBehaviour
    {
        private void Awake()
        {
            TexturesManager texturesManager = GetComponent<TexturesManager>();
            texturesManager.idGetterController = new IdController();
        }
    }

}