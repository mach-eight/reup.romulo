using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ReupVirtualTwin.enums;


namespace ReupVirtualTwin.editor
{
    public class AppProxy : EditorWindow
    {
        bool viewControls = false;
        IWebMessageReceiver webMessageReceiver;

        [MenuItem("Reup Romulo/App proxy")]
        public static void ShowWindow()
        {
            GetWindow<AppProxy>("App proxy");
        }
        void OnGUI()
        {
            GetDependencies();
            viewControls = EditorGUILayout.Foldout(viewControls, "View controls");
            if (viewControls)
            {
                ViewControls();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        void GetDependencies()
        {
            if (Application.isPlaying && webMessageReceiver == null)
            {
                webMessageReceiver = ObjectFinder.FindWebMessageReceiver();
            }
        }
        void ViewControls()
        {
            if (GUILayout.Button("Activate dollhouse view"))
            {
                string message = JsonConvert.SerializeObject(new JObject
                {
                    { "type", WebMessageType.activateViewMode },
                    { "payload", new JObject
                        {
                            { "viewMode", ViewMode.dollHouse.ToString() },
                            { "requestId", "UUID1" },
                        }
                    }
                });
                webMessageReceiver.ReceiveWebMessage(message);
            }
            if (GUILayout.Button("Activate first person view"))
            {
                string message = JsonConvert.SerializeObject(new JObject
                {
                    { "type", WebMessageType.activateViewMode },
                    { "payload", new JObject
                        {
                            { "viewMode", ViewMode.firstPerson.ToString() },
                            { "requestId", "UUID2" },
                        }
                    }
                });
                webMessageReceiver.ReceiveWebMessage(message);
            }
        }
    }
}
