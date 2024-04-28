using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    [CreateAssetMenu(fileName = "Color", menuName ="Color")]
    public class ColorSO : ScriptableObject
    {
        public Color color;
    }
}