using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Mesh Data", menuName = "ScriptableObjects/MeshData", order = 1)]
public class MeshAsset : ScriptableObject
{
    [Tooltip("NAME == MESH")]
    [SerializeField] public string meshName;

    [Space]
    [SerializeField] public bool generateCollider;
    [Space]
    [SerializeField] public Vector3[] vertices;
    [Space]
    [SerializeField] public Vector2[] uv;
    [Space]
    [SerializeField] public int subMeshCount;
    [Space]
    [SerializeField] public NestedArrayInt[] trianglesSubMesh;
    [Space]
    [SerializeField] public NestedArrayInt[] segmentUV;
    [Space]
    [SerializeField] public Material[] materials;
}

