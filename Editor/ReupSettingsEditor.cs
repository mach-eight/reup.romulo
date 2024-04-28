using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReupVirtualTwin.editor
{
    public class ReupSettingsEditor : EditorWindow
    {
       [MenuItem( "Reup Romulo/Reup Romulo Settings" )]
        public static void ShowWindow()
        {
            GetWindow<ReupSettingsEditor>("Reup Romulo Settings");
        }
        void OnGUI()
        { 
            if (GUILayout.Button("Add shaders to webgl build"))
            {
                AddShaders();
            }
        }
        void AddShaders()
        {
            AddShaderUtil.AddAlwaysIncludedShader("sHTiF/HandleShader");
            AddShaderUtil.AddAlwaysIncludedShader("sHTiF/AdvancedHandleShader");
        }
    }
}
