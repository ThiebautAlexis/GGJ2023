using System;
using UnityEditor; 
using UnityEngine; 

namespace GGJ2023.Editor
{
    [CustomEditor(typeof(GridData))]
    public class GridDataEditor : UnityEditor.Editor
    {
        private static readonly string xLengthPropertyName = "xLength";
        private static readonly string yLengthPropertyName = "yLength";
        private static readonly string gridPropertyName = "cells";
        private static readonly Color rootColor = Color.red;  

        #region Fields and Properties
        private SerializedProperty xLengthProperty = null; 
        private SerializedProperty yLengthProperty = null;
        private SerializedProperty gridProperty = null;
        #endregion

        #region Methods
        private void OnEnable()
        {
            xLengthProperty = serializedObject.FindProperty(xLengthPropertyName); 
            yLengthProperty = serializedObject.FindProperty(yLengthPropertyName);
            gridProperty = serializedObject.FindProperty(gridPropertyName); 
            
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck(); 
            EditorGUILayout.PropertyField(xLengthProperty, new GUIContent("Grid Width", "The Width of the Grid")); 
            EditorGUILayout.PropertyField(yLengthProperty, new GUIContent("Grid Height", "The Height of the Grid"));
            if(EditorGUI.EndChangeCheck())
            {
                gridProperty.arraySize = xLengthProperty.intValue * yLengthProperty.intValue;
                serializedObject.ApplyModifiedProperties(); 
            }

            GUILayout.Label(new GUIContent("GRID"));
            Color _backgroundColor = GUI.backgroundColor; 
            EditorGUI.BeginChangeCheck();
            for (int y = 0; y < yLengthProperty.intValue; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < xLengthProperty.intValue; x++)
                {
                    if (gridProperty.GetArrayElementAtIndex(x + (y * xLengthProperty.intValue)).intValue == (int)CellState.Root)
                        GUI.backgroundColor = rootColor; 
                    EditorGUILayout.PropertyField(gridProperty.GetArrayElementAtIndex(x + (y * xLengthProperty.intValue)), new GUIContent(string.Empty), GUILayout.Width(Screen.width / xLengthProperty.intValue));
                    GUI.backgroundColor = _backgroundColor;     
                }
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion 

    }
}
