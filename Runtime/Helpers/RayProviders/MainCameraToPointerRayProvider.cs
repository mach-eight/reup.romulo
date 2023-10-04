﻿using System;
using UnityEngine;
public class MainCameraToPointerRayProvider : MonoBehaviour, IRayProvider
{
    private InputProvider _inputProvider;

    private void Start()
    {
        // todo: is it really necessary to create an instance?
        // can you make all the input methods statics instead?
        _inputProvider = new InputProvider();
    }

    public Ray GetRay()
    {
        return Camera.main.ScreenPointToRay(_inputProvider.PointerInput());
	}
}

