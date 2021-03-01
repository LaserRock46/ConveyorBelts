using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Mesh Data", menuName = "ScriptableObjects/MeshData", order = 1)]
public class MeshAsset : ScriptableObject
{
    #region Mesh Asset Data
    [Space]
    public Vector3[] ogVertices;
    [Space]
    public int[] ogTriangles;
    [Space]
    public Vector2[] ogUvs;
    [Space]
    public Color32[] ogVertexColors;
    [Space]
    public int subMeshCount;
    [Space]
    public NestedArrayInt[] trianglesSubMesh;
    [Space]
    public UvSegment[] uvSegments;
    /// refactored
    public bool generateCollider;
    [Space]
    public int loopCount = 128;
    [Space]
    public Material[] materials;
    [Space]
    public EdgeLoop edgeLoop;
    [Space]
    public int[] precomputedTriangles;


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

