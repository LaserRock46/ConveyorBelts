using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Mesh Data", menuName = "ScriptableObjects/MeshData", order = 1)]
public class MeshAsset : ScriptableObject
{
    #region Mesh Asset Data
    public bool generateCollider;
    [Space]
    public int loopCount = 128;
    [Space]
    public Material[] materials;
    [Space]
    public EdgeLoop edgeLoop;
    [Space]
    public int[] precomputedTriangles;
    [Space]
    public UvMapOrientation uvMapOrientation;
    public enum UvMapOrientation { ForwardX,ForwardY}
    [Space]
    public float compensateForwardStretch = 1;
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

