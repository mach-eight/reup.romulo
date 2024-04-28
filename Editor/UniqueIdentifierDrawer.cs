using UnityEditor;
using UnityEngine;
using System;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.editor
{
	[CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
	public class UniqueIdentifierDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			// Place a label so it can't be edited by accident
			Rect textFieldPosition = position;
			textFieldPosition.height = 16;
			DrawLabelField(textFieldPosition, prop, label);
		}

		void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
		}
	}
}
