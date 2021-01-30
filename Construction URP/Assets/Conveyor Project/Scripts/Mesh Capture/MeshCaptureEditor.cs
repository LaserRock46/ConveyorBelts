using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MeshCapture))]
public class MeshCaptureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        MeshCapture myScript = (MeshCapture)target;
        if (GUILayout.Button("Get Data"))
        {
            myScript.GetData();
        }
    }
}
