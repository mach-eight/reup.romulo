using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReupVirtualTwin.helpers
{
    public class ImageColor : MonoBehaviour
    {
        public ColorSO colorSo;

        private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
            image.color = colorSo.color;
        }
    }
}