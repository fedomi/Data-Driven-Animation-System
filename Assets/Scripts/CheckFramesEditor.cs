using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CheckFrames)), CanEditMultipleObjects]
public class CheckFramesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        CheckFrames myScript = (CheckFrames)target;
        if (GUILayout.Button("Delete Clip"))
        {
            myScript.DeleteClip();
        }
        if (GUILayout.Button("Export"))
        {
            myScript.ExportData();
        }
    }
}

