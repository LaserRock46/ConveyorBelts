using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MeshAsset))]

public class MeshCaptureEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        MeshAsset myScript = (MeshAsset)target;
        if (GUILayout.Button("Get Data"))
        {
            myScript.GetData();
            EditorUtility.SetDirty(target);
        }
    }
    
}
