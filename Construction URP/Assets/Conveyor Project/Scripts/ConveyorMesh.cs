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
    [SerializeField] private MeshAsset[] _meshAssets;
    public int currentMeshAssetIndex;
    private OrientedPoint _orientedPoints;
    [SerializeField] private Transform _previewTransform;
    [SerializeField] private MeshFilter _previewMeshFilter;
    [SerializeField] private MeshRenderer _previewMeshRenderer;
    [Range(0,255)] public byte conveyorSpeed = 50;
    //public Mesh mesh;


    #endregion

    #region Functions
    public Mesh GetMeshSubmesh0(Mesh newMesh, Vector3[] vertices, Vector2[] uvs, int[] triangles, Color32[] vertexColors)
    {
      
        newMesh.SetVertices(vertices);
        newMesh.SetUVs(0, uvs);
        newMesh.SetTriangles(triangles, 0);
        newMesh.SetColors(vertexColors);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        //newMesh.RecalculateUVDistributionMetric(0);
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

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount - 1; currentEdgeLoop++) // loop through all new mesh segments
        {
            int targetSegmentIndexStart = currentEdgeLoop * ogVerticesLength;
            for (int currentOgVertex = 0; currentOgVertex < originalSegmentVertices.Length; currentOgVertex++) //loop through all original mesh vertices
            {
                int targetVertexIndex = targetSegmentIndexStart + currentOgVertex;
                // each segment are mede from two edge loops, each edge loops vertices positions are based on current(back loop) or next(front loop) oriented point taken from bezier

                //front loop
                if (IsOriginalVertexInFront(originalSegmentVertices[currentOgVertex]))
                {
                    newVertices[targetVertexIndex] = _orientedPoints.GetPointInLocalSpace(originalSegmentVertices[currentOgVertex], currentEdgeLoop + 1);
                }
                // back loop
                else
                {
                    newVertices[targetVertexIndex] = _orientedPoints.GetPointInLocalSpace(originalSegmentVertices[currentOgVertex], currentEdgeLoop);
                } 
            }
        }
        return newVertices;
    }
    Vector2[] GetUVs(int edgeLoopCount,Vector2[] originalUVs, UvSegment[] segmentUV, float[] segmentDistanceForward, float[] segmentDistanceBackward, bool reverse)
    {
        int newUVsLength = edgeLoopCount * originalUVs.Length;
        Vector2[] newUVs = new Vector2[newUVsLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount - 1; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * originalUVs.Length;
            for (int currentSegmentUV = 0; currentSegmentUV < segmentUV.Length; currentSegmentUV++)
            {             
                Vector2 uvVectorStart = new Vector2(segmentDistanceForward[currentEdgeLoop], originalUVs[segmentUV[currentSegmentUV].start].y);
                Vector2 uvVectorEnd = new Vector2(segmentDistanceForward[currentEdgeLoop + 1], originalUVs[segmentUV[currentSegmentUV].end].y);

                int start = targetSegmentIndexStart + segmentUV[currentEdgeLoop].start;
                int end = targetSegmentIndexStart +  segmentUV[currentEdgeLoop].end;

                newUVs[start] = uvVectorStart;
                newUVs[end] = uvVectorEnd;
            }
        }
        return newUVs;
    }
     
    int[] GetTriangles(int edgeLoopCount,int originalTrianglesLength,int[] meshAssetTriangles)
    {
        int newTrianglesLength = edgeLoopCount * originalTrianglesLength;
        int[] newTriangles = new int[newTrianglesLength];
        for (int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] = meshAssetTriangles[i];
        }
        return newTriangles;
    }
    bool IsVertexScrollable(Color32 vertexColor)
    {
        return vertexColor.g != 0;
    }
    Color32[] GetVertexColors(int edgeLoopCount, Color32[] originalVertexColors, byte newConveyorSpeed)
    {
        int newVertexColorsLength = edgeLoopCount * originalVertexColors.Length;
        Color32[] newVertexColors = new Color32[newVertexColorsLength];
        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount - 1; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * originalVertexColors.Length;
            for (int currentOgVertex = 0; currentOgVertex < originalVertexColors.Length; currentOgVertex++)
            {
                int targetVertexIndex = targetSegmentIndexStart + currentOgVertex;
                if (IsVertexScrollable(originalVertexColors[currentOgVertex]))
                {
                    newVertexColors[targetVertexIndex] = new Color32(0, newConveyorSpeed, 0, 0);
                }
            }
        }
        return newVertexColors;
    }
    #endregion



    #region Methods
    public void AssignOrientedPoints(OrientedPoint orientedPoints)
    {
        _orientedPoints = orientedPoints;
    }
    public void MeshUpdate(bool reverse)
    {
        if (_orientedPoints.positions.Length == 0) return;
        int targetEdgeLoopCount = _orientedPoints.positions.Length;

        Vector3[] vertices = GetExtrudedMeshVertices(targetEdgeLoopCount,_meshAssets[currentMeshAssetIndex].ogVertices);
        Vector2[] uvs = GetUVs(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogUvs, _meshAssets[currentMeshAssetIndex].uvSegments,_orientedPoints.segmentDistanceForward,_orientedPoints.segmentDistanceBackward,reverse);
        int[] triangles = GetTriangles(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogTriangles.Length, _meshAssets[currentMeshAssetIndex].trianglesSubMesh[0].value);
        Color32[] vertexColors = GetVertexColors(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogVertexColors,conveyorSpeed);

        _previewMeshFilter.mesh = GetMeshSubmesh0(_previewMeshFilter.mesh, vertices,uvs,triangles,vertexColors);
    }
    #endregion

}
