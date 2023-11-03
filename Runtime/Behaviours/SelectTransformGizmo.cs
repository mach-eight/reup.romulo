
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;

//Frist import the package then
//Create a empty gameobject called SelectTransformGizmo and attach this gameobject to it

public class SelectTransformGizmo : MonoBehaviour
{
    public Material highlightMaterial;
    public Material selectionMaterial;

    private Material originalMaterialHighlight;
    private Material originalMaterialSelection;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitHandle;
    private GameObject runtimeTransformGameObj;
    private RuntimeTransformHandle runtimeTransformHandle;
    private int runtimeTransformLayer = 6;
    private int runtimeTransformLayerMask;

    private void Start()
    {
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; //Layer number represented by a single bit in the 32-bit integer using bit shift
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;
        runtimeTransformGameObj.SetActive(false);
    }

    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.GetComponent<MeshRenderer>().sharedMaterial = originalMaterialHighlight;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            
            highlight = raycastHit.transform;
            print(highlight);
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
                {
                    originalMaterialHighlight = highlight.GetComponent<MeshRenderer>().material;
                    highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            
            ApplyLayerToChildren(runtimeTransformGameObj);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (Physics.Raycast(ray, out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) //Raycast towards runtime transform handle only
                {
                }
                else if (highlight)
                {
                    if (selection != null)
                    {
                        selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                    }
                    selection = raycastHit.transform;
                    if (selection.GetComponent<MeshRenderer>().material != selectionMaterial)
                    {
                        originalMaterialSelection = originalMaterialHighlight;
                        selection.GetComponent<MeshRenderer>().material = selectionMaterial;
                        runtimeTransformHandle.target = selection;
                        runtimeTransformGameObj.SetActive(true);
                    }
                    highlight = null;
                }
                else
                {
                    if (selection)
                    {
                        selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                        selection = null;

                        runtimeTransformGameObj.SetActive(false);
                    }
                }
            }
            else
            {
                if (selection)
                {
                    selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                    selection = null;

                    runtimeTransformGameObj.SetActive(false);
                }
            }
        }

        //Hot Keys for move, rotate, scale, local and Global/World transform
        if (runtimeTransformGameObj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                runtimeTransformHandle.type = HandleType.POSITION;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                runtimeTransformHandle.type = HandleType.ROTATION;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                runtimeTransformHandle.type = HandleType.SCALE;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    runtimeTransformHandle.space = HandleSpace.WORLD;
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    runtimeTransformHandle.space = HandleSpace.LOCAL;
                }
            }
        }

    }


    private void ApplyLayerToChildren(GameObject parentGameObj)
    {
        foreach (Transform transform1 in parentGameObj.transform)
        {
            int layer = parentGameObj.layer;
            transform1.gameObject.layer = layer;
            foreach (Transform transform2 in transform1)
            {
                transform2.gameObject.layer = layer;
                foreach (Transform transform3 in transform2)
                {
                    transform3.gameObject.layer = layer;
                    foreach (Transform transform4 in transform3)
                    {
                        transform4.gameObject.layer = layer;
                        foreach (Transform transform5 in transform4)
                        {
                            transform5.gameObject.layer = layer;
                        }
                    }
                }
            }
        }
    }

}