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
    [Range(0,255)] public byte conveyorSpeed = 50;
    public Mesh mesh;


    #endregion

    #region Functions
    public Mesh GetMeshSubmesh0(Vector3[] vertices, Vector2[] uvs, int[] triangles, Color32[] vertexColors)
    {
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertices);
        newMesh.SetUVs(0, uvs);
        newMesh.SetTriangles(triangles, 0);
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
                    newVertices[targetVertexIndex] = _bezier.orientedPoints.WorldToLocal(originalSegmentVertices[currentOgVertex], currentEdgeLoop + 1);
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
                    newUVs[targetUVsIndex] = new Vector2();
                }
            }
        }
        return newUVs;
    }

    Vector2[] GetUVs(int edgeLoopCount,Vector2[] originalSegmentUVs, NestedArrayInt[] segmentUV, float[] segmentDistance, bool reverse)
    {
        int newUVsLength = edgeLoopCount * originalSegmentUVs.Length;
        Vector2[] newUVs = new Vector2[newUVsLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * originalSegmentUVs.Length;
            for (int currentSegmentUV = 0; currentSegmentUV < segmentUV.Length; currentSegmentUV++)
            {             
                Vector2 uvVectorStart = new Vector2(originalSegmentUVs[segmentUV[currentSegmentUV].value[0]].x,  segmentDistance[currentEdgeLoop]);
                Vector2 uvVectorEnd = new Vector2(originalSegmentUVs[segmentUV[currentSegmentUV].value[1]].x, segmentDistance[currentEdgeLoop + 1]);

                int start = targetSegmentIndexStart + segmentUV[currentEdgeLoop].value[0];
                int end = targetSegmentIndexStart +  segmentUV[currentEdgeLoop].value[1];

                newUVs[start] = uvVectorStart;
                newUVs[end] = uvVectorEnd;
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
    Color32[] GetVertexColors(int edgeLoopCount, Color32[] originalVertexColors, byte newConveyorSpeed)
    {
        int newVertexColorsLength = edgeLoopCount * originalVertexColors.Length;
        Color32[] newVertexColors = new Color32[newVertexColorsLength];
        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * originalVertexColors.Length;
            for (int currentOgVertex = 0; currentOgVertex < originalVertexColors.Length; currentOgVertex++)
            {
                int targetVertexIndex = targetSegmentIndexStart + currentOgVertex;
                if (originalVertexColors[currentOgVertex].g != 0)
                {
                    newVertexColors[targetSegmentIndexStart] = new Color32(0, newConveyorSpeed, 0, 0);
                }
            }
        }
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

        Vector3[] vertices = GetExtrudedMeshVertices(targetEdgeLoopCount,_meshAssets[currentMeshAssetIndex].ogVertices);
        Vector2[] uvs = GetUVs(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogUvs, _meshAssets[currentMeshAssetIndex].segmentUV,_bezier.segmentDistance,reverse);
        int[] triangles = GetTriangles(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogVertices.Length, _meshAssets[currentMeshAssetIndex].trianglesSubMesh[0].value);
        Color32[] vertexColors = GetVertexColors(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogVertexColors,conveyorSpeed);

        _previewMeshFilter.mesh = GetMeshSubmesh0(vertices,uvs,triangles,vertexColors);
    }
    #endregion

}
