using UnityEditor;
using UnityMeshSimplifier;
using System;
using TB;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.editor
{
    public class SimplifyMesh : EditorWindow
    {



        [MenuItem("Reup Romulo/Simplify Mesh")]
        public static void ShowWindow()
        {
            GetWindow<SimplifyMesh>("Simplify Mesh");
        }

        void OnGUI()
        {
        }

    }
}
