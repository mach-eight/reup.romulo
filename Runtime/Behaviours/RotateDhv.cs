using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class RotateDhv : MonoBehaviour
{
    [SerializeField] public GameObject thisCamera;
    [SerializeField] public GameObject otherVirtualCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeCamera();
        }
    }

    private void ChangeCamera()
    {
        otherVirtualCamera.SetActive(true);
        thisCamera.SetActive(false);
    }
}
