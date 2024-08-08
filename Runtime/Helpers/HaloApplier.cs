using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helperInterfaces;

namespace ReupVirtualTwin.helpers
{
    public class HaloApplier : IObjectHighlighter
    {
        public Color emissionColor = Color.white;
        public float emissionIntensity = 0.01f;
        public void HighlightObject(GameObject obj)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                if (material != null)
                {
                    Color finalColor = emissionColor * Mathf.LinearToGammaSpace(emissionIntensity);
                    material.SetColor("_EmissionColor", finalColor);
                    material.EnableKeyword("_EMISSION");
                }
            }
            foreach (Transform child in obj.transform)
            {
                HighlightObject(child.gameObject);
            }
        }

        public void UnhighlightObject(GameObject obj)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                if (material != null)
                {
                    material.DisableKeyword("_EMISSION");
                }
            }
            foreach (Transform child in obj.transform)
            {
                UnhighlightObject(child.gameObject);
            }
        }
    }
}
