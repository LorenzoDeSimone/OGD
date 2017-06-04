﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Platform))]
public class MapManager : Editor
{
    private int num;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Platform myScript = (Platform)target;
        if (GUILayout.Button("Connect Platforms"))
        {
            myScript.connectPlatform();
        }
    }
}
#endif