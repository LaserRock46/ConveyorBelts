using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorMesh
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private MeshAsset[] _meshAssets;
    public int currentMeshAssetIndex;
    private Bezier _bezier;
    [SerializeField] private MeshFilter _previewMeshFilter;
    [SerializeField] private MeshRenderer _previewMeshRenderer;
    public Mesh mesh;

    
    #endregion

    #region Functions
    public Mesh GetMeshSubmesh0(Vector3[] vertices, Vector2[] uvs, int[] triangles, Color32[] vertexColors)
    {
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertices);
        newMesh.SetUVs(0,uvs);
        newMesh.SetTriangles(triangles,0);
        newMesh.SetColors(vertexColors);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        newMesh.RecalculateUVDistributionMetric(0);
        return newMesh;
    }
    bool IsOriginalVertexInFront(Vector3 originalVertex)
    {
        if (originalVertex.z >= 0) 
        {
            return true;
        }
        return false;
    }
    Vector3[] GetExtrudedMeshVertices(int edgeLoopCount, Vector3[] originalSegmentVertices)
    {
        int newVerticesLength = edgeLoopCount * originalSegmentVertices.Length; // total new mesh vertices length
        int ogVerticesLength = originalSegmentVertices.Length;
        Vector3[] newVertices = new Vector3[newVerticesLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount; currentEdgeLoop++) // loop through all new mesh segments
        {
            int targetSegmentIndexStart = currentEdgeLoop * ogVerticesLength;
            for (int currentOgVertex = 0; currentOgVertex < originalSegmentVertices.Length; currentOgVertex++) //loop through all original mesh vertices
            {
                int targetVertexIndex = targetSegmentIndexStart + currentOgVertex;
                // each segment are mede from two edge loops, each edge loops vertices positions are based on current(back loop) or next(front loop) oriented point taken from bezier
                
                //front loop
                if (IsOriginalVertexInFront(originalSegmentVertices[currentOgVertex]))
                {
                    newVertices[targetVertexIndex] = _bezier.orientedPoints.WorldToLocal(originalSegmentVertices[currentOgVertex],currentEdgeLoop + 1);
                }
                // back loop
                else
                {
                    newVertices[targetVertexIndex] = _bezier.orientedPoints.WorldToLocal(originalSegmentVertices[currentOgVertex], currentEdgeLoop);
                }
            }
        }
        return newVertices;
    }
    bool IsOriginalUVStart(Vector2 originalUV)
    {
        if (originalUV.y <= 0)
        {
            return true;
        }
        return false;
    }
    Vector2[] GetExtrudedMeshUVs(int edgeLoopCount, Vector2[] originalSegmentUVs, bool reverse)
    {
        int newUVsLength = edgeLoopCount * originalSegmentUVs.Length;
        int ogUVsLength = originalSegmentUVs.Length;
        Vector2[] newUVs = new Vector2[newUVsLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * ogUVsLength;
            for (int currentOgUVs = 0; currentOgUVs < originalSegmentUVs.Length; currentOgUVs++)
            {
                int targetUVsIndex = targetSegmentIndexStart + currentOgUVs;
                if (IsOriginalUVStart(originalSegmentUVs[currentOgUVs]))
                {
                    newUVs[targetUVsIndex] = new Vector2();
                }
                else
                {
                    newUVs[targetUVsIndex] =new Vector2();
                }
            }
        }
        return newUVs;
    }
    int[] GetTriangles(int edgeLoopCount,int originalVerticesLength,int[] meshAssetTriangles)
    {
        int newTrianglesLength = edgeLoopCount * originalVerticesLength;
        int[] newTriangles = new int[newTrianglesLength];
        for (int i = 0; i < newTrianglesLength; i++)
        {
            newTriangles[i] = meshAssetTriangles[i];
        }
        return newTriangles;
    }
    Color32[] GetVertexColors(int edgeLoopCount, int originalVerticesLength,Color32[] originalVertexColors,int newConveyorSpeed) 
    {
        int newVertexColorsLength = edgeLoopCount * originalVerticesLength;
        Color32[] newVertexColors = new Color32[newVertexColorsLength];
        return newVertexColors;
    }
    #endregion



    #region Methods
    public void AssignBezier(Bezier bezier)
    {
        _bezier = bezier;
    }
    public void MeshUpdate(bool reverse)
    {
        int targetEdgeLoopCount = _bezier.pointCount;

    }
    #endregion

}
