using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.models;
using System;
using ReupVirtualTwin.modelInterfaces;


namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(SpacesRecord))]
    public class SpacesRecordEditor : Editor
    {
        bool CheckForStaleList<T>(List<T> list, int expectedLength)
        {
            if (list == null)
            {
                return true;
            }
            if (list.Count != expectedLength)
            {
                return true;
            }
            if (list.Count > 0 && list[0] == null)
            {
                return true;
            }
            return false;
        }

        public override void OnInspectorGUI()
        {
            SpacesRecord spacesRecord = (SpacesRecord)target;
            if (!SpaceTagIsDefined())
            {
                EditorGUILayout.HelpBox("No defined space selector tag yet", MessageType.Warning);
                return;
            }
            GameObject[] spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            if (CheckForStaleList<ISpaceJumpPoint>(spacesRecord.jumpPoints, spaces.Length))
            {
                spacesRecord.UpdateSpaces();
            }
            EditorGUILayout.LabelField("List of spaces in the scene: ", EditorStyles.boldLabel);
            foreach (SpaceJumpPoint spaceSelector in spacesRecord.jumpPoints)
            {
                EditorGUILayout.LabelField($" - {spaceSelector.gameObject.name} ({spaceSelector.spaceName})");
            }
            DrawDefaultInspector();
        }

        bool SpaceTagIsDefined()
        {
            return Array.Exists(
                UnityEditorInternal.InternalEditorUtility.tags,
                element => element == TagsEnum.spaceSelector
            );
        }

    }
}
