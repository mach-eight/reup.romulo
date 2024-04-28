using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.models;


namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(SpacesRecord))]
    public class SpacesRecordEditor : Editor
    {

        bool ListCheck<T>(List<T> list, int expectedLength)
        {
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
            GameObject[] spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            if (ListCheck<SpaceJumpPoint>(spacesRecord.jumpPoints, spaces.Length))
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
    }
}
