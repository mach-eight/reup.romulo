using System.Linq;
using UnityEditor;
using UnityEngine;
using ReupVirtualTwin.models;

[CustomEditor(typeof(MaterialSelectionTrigger))]
public class MaterialSelectionTriggerEditor : Editor
{
    MaterialSelectionTrigger trigger;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        trigger = (MaterialSelectionTrigger)target;

        if (trigger.objects.Count != 0)
        {
            if (trigger.submeshIndexes.Length != trigger.objects.Count)
                trigger.submeshIndexes = new int[trigger.objects.Count];
            EditorGUI.BeginChangeCheck();
            foreach (var (obj, j) in trigger.objects.Select((v, i) => (v, i)))
            {
                if (obj != null)
                {
                    var materials = obj.GetComponent<Renderer>().sharedMaterials;
                    if (materials.Length > 1)
                    {
                        var warningMessage = $"The object '{obj.name}' has more than one material assigned, select the material you want to change";
                        EditorGUILayout.HelpBox(warningMessage, MessageType.Warning, true);
                        string[] materialOptions = new string[materials.Length];
                        for (int i = 0; i < materialOptions.Length; i++)
                        {
                            materialOptions[i] = materials[i].name;
                        }
                        var infoMessage = $"Material to change for {obj.name}";
                        trigger.submeshIndexes[j] = EditorGUILayout.Popup(infoMessage,
                                                                                  trigger.submeshIndexes[j],
                                                                                  materialOptions);
                    }
                    checkIndexOutOfMaterialsArray(j, materials);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(trigger);
            }
        }
    }
    private void checkIndexOutOfMaterialsArray(int index, Material[] materials)
    {
        if (trigger.submeshIndexes[index] >= materials.Length)
        {
            trigger.submeshIndexes[index] = 0;
        }
    }
}
