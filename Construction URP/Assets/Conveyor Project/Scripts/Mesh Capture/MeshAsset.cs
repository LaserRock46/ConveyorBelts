﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Mesh Data", menuName = "ScriptableObjects/MeshData", order = 1)]
public class MeshAsset : ScriptableObject
{
    #region Mesh Asset Data
    [SerializeField] public bool generateCollider;
    [Space]
    [SerializeField] public int loopCount = 128;
    [Space]
    [SerializeField] public Vector3[] ogVertices;
    [Space]
    [SerializeField] public int[] ogTriangles;
    [Space]
    [SerializeField] public Vector2[] ogUvs;
    [Space]
    [SerializeField] public Color32[] ogVertexColors;
    [Space]
    [SerializeField] public int subMeshCount;
    [Space]
    [SerializeField] public NestedArrayInt[] trianglesSubMesh;
    [Space]
    [SerializeField] public UvSegment[] uvSegments;
    [Space]
    [SerializeField] public Material[] materials;
    #endregion
    [Space]
    [Header("Bake Data Source")]
    public MeshCapture meshCapture;
    [ContextMenu("Get Data")]
    public void GetData()
    {    
        meshCapture.GetData(this);   
    }
  
    

}

