using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorMesh
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    [SerializeField] private bool _drawMesh;
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

        newMesh.Clear();
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
                // each segment are mede from two edge loops, each edge loops vertices positions are based on current(back loop) or next(front loop) oriented point taken from points array

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
    bool IsEven(int number)
    {
        return number % 2 == 0;
    }
    Vector3[] GetExtrudedMeshVerticesAlternative(int edgeLoopCount, Vector3[] originalSegmentVertices)
    {
        int ogVerticesHalfLength = originalSegmentVertices.Length/2;
        int newVerticesLength = edgeLoopCount * ogVerticesHalfLength; 
        Vector3[] newVertices = new Vector3[newVerticesLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount - 1; currentEdgeLoop++) 
        {
            int targetSegmentIndexStart = currentEdgeLoop * ogVerticesHalfLength;
            for (int currentOgVertex = 0; currentOgVertex < originalSegmentVertices.Length; currentOgVertex++) 
            {
                int targetVertexIndex = targetSegmentIndexStart + currentOgVertex;

                if (IsEven(currentEdgeLoop))
                {
                    if (!IsOriginalVertexInFront(originalSegmentVertices[currentOgVertex]))
                    {
                        newVertices[targetVertexIndex] = _orientedPoints.GetPointInLocalSpace(originalSegmentVertices[currentOgVertex], currentEdgeLoop);
                    }
                }
                else
                {
                    if (IsOriginalVertexInFront(originalSegmentVertices[currentOgVertex]))
                    {
                        newVertices[targetVertexIndex] = _orientedPoints.GetPointInLocalSpace(originalSegmentVertices[currentOgVertex], currentEdgeLoop);
                    }
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
    Vector2[] GetUVsAlternative(int edgeLoopCount, Vector2[] originalUVs, UvSegment[] segmentUV, float[] segmentDistanceForward, float[] segmentDistanceBackward, bool reverse)
    {
        int ogUvsHalfLength = originalUVs.Length / 2;
        int newUVsLength = edgeLoopCount * ogUvsHalfLength;
        Vector2[] newUVs = new Vector2[newUVsLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount - 1; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * ogUvsHalfLength;
            for (int currentSegmentUV = 0; currentSegmentUV < segmentUV.Length; currentSegmentUV++)
            {
                if (IsEven(currentEdgeLoop))
                {
                    Vector2 uvVectorStart = new Vector2(segmentDistanceForward[currentEdgeLoop], originalUVs[segmentUV[currentSegmentUV].start].y);
                    int start = targetSegmentIndexStart + segmentUV[currentEdgeLoop].start;
                    newUVs[start] = uvVectorStart;
                }
                else
                {
                    int end = targetSegmentIndexStart + segmentUV[currentEdgeLoop].end;
                    Vector2 uvVectorEnd = new Vector2(segmentDistanceForward[currentEdgeLoop], originalUVs[segmentUV[currentSegmentUV].end].y);
                    newUVs[end] = uvVectorEnd;
                }
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
    int[] GetTrianglesAlternative(int edgeLoopCount, int originalTrianglesLength, int[] meshAssetTriangles)
    {
        int newTrianglesLength = (edgeLoopCount - 1) * (originalTrianglesLength/2);
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
    Color32[] GetVertexColorsAlternative(int edgeLoopCount, Color32[] originalVertexColors, byte newConveyorSpeed)
    {
        int ogVertexColorHalfLength = originalVertexColors.Length / 2;
        int newVertexColorsLength = edgeLoopCount * ogVertexColorHalfLength;
        Color32[] newVertexColors = new Color32[newVertexColorsLength];
        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount - 1; currentEdgeLoop++)
        {
            int targetSegmentIndexStart = currentEdgeLoop * ogVertexColorHalfLength;
            for (int currentOgVertex = 0; currentOgVertex < originalVertexColors.Length; currentOgVertex++)
            {
                int targetVertexIndex = targetSegmentIndexStart + currentOgVertex;

                if (IsVertexScrollable(originalVertexColors[currentOgVertex]))
                {
                    if (IsEven(currentEdgeLoop))
                    {
                        newVertexColors[targetVertexIndex] = new Color32(0, newConveyorSpeed, 0, 0);
                    }
                    else
                    {
                        newVertexColors[targetVertexIndex] = new Color32(0, newConveyorSpeed, 0, 0);
                    }
                }
            }
        }
        return newVertexColors;
    }
    //refactored
    int[] GetTrimmedPrecomputedTriangles(int edgeLoopCount, EdgeLoop edgeLoop, int[] precomputedTriangles)
    {
        int trianglesLength = edgeLoopCount * edgeLoop.edges.Length * 6;
        int[] trimmedPrecomputedTriangles = new int[trianglesLength];

        for (int i = 0; i < trimmedPrecomputedTriangles.Length; i++)
        {
            trimmedPrecomputedTriangles[i] = precomputedTriangles[i];
        }
        return trimmedPrecomputedTriangles;
    }
    (Vector3[] vertices,Vector2[] uvs, Color32[] vertexColors) GetVertexDataArray(int edgeLoopCount, OrientedPoint orientedPoints, EdgeLoop edgeLoop, bool reversedUV, MeshAsset.UvMapOrientation uvMapOrientation)
    {
        int newVertexDataArrayLength = edgeLoopCount * edgeLoop.vertexDatas.Length;
        Vector3[] vertices = new Vector3[newVertexDataArrayLength];
        Vector2[] uvs = new Vector2[newVertexDataArrayLength];
        Color32[] vertexColors = new Color32[newVertexDataArrayLength];

        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount; currentEdgeLoop++)
        {
            int currentLoopArrayIndex = currentEdgeLoop * edgeLoop.vertexDatas.Length;

            for (int currentVertexData = 0; currentVertexData < edgeLoop.vertexDatas.Length; currentVertexData++)
            {
                int currentVertexDataIndex = currentLoopArrayIndex + currentVertexData;

               
            }
        }

        return (vertices, uvs, vertexColors);
    }
    Vector3 GetVertex(int currentEdgeLoop, VertexData vertexData, OrientedPoint orientedPoints)
    {
        return orientedPoints.GetPointInLocalSpace(vertexData.vertexPosition,currentEdgeLoop);
    }
    Vector2 GetUV(int currentEdgeLoop, VertexData vertexData, bool reversedUV, OrientedPoint orientedPoints, MeshAsset.UvMapOrientation uvMapOrientation)
    {
        float forwardLength = 0;
        float sidewardLength = 0;
        Vector2 uv = new Vector2();

        if (uvMapOrientation == MeshAsset.UvMapOrientation.ForwardX)
        {         
            forwardLength = vertexData.vertexUV.x < 0 ?orientedPoints.segmentDistanceBackward[currentEdgeLoop]: orientedPoints.segmentDistanceForward[currentEdgeLoop];
            sidewardLength = vertexData.vertexUV.y;
            uv = new Vector2(forwardLength, sidewardLength);
        }
        else
        {
            forwardLength = vertexData.vertexUV.y < 0 ? orientedPoints.segmentDistanceBackward[currentEdgeLoop] : orientedPoints.segmentDistanceForward[currentEdgeLoop];
            sidewardLength = vertexData.vertexUV.x;
            uv = new Vector2(sidewardLength, forwardLength);
        }
        return uv;
    }
    Color32 GetVertexColor(VertexData vertexData, byte newConveyorSpeed)
    {
        Color32 vertexColor = new Color32();
        if (IsVertexScrollable(vertexData.vertexColor))
        {
             vertexColor = new Color32(vertexData.vertexColor.r, newConveyorSpeed, vertexData.vertexColor.b, vertexData.vertexColor.a);
        }
        else
        {
            vertexColor = vertexData.vertexColor;
        }
        return vertexColor;
    }
    #endregion



    #region Methods
    public void AssignOrientedPoints(OrientedPoint orientedPoints)
    {
        _orientedPoints = orientedPoints;
    }
    public void MeshUpdate(bool reverse)
    {
        if (!_drawMesh) return;
        if (_orientedPoints.positions.Length == 0) return;

        int targetEdgeLoopCount = _orientedPoints.positions.Length;

        Vector3[] vertices = GetExtrudedMeshVerticesAlternative(targetEdgeLoopCount,_meshAssets[currentMeshAssetIndex].ogVertices);
        Vector2[] uvs = GetUVsAlternative(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogUvs, _meshAssets[currentMeshAssetIndex].uvSegments,_orientedPoints.segmentDistanceForward,_orientedPoints.segmentDistanceBackward,reverse);
        Color32[] vertexColors = GetVertexColorsAlternative(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogVertexColors,conveyorSpeed);
        int[] triangles = GetTrianglesAlternative(targetEdgeLoopCount, _meshAssets[currentMeshAssetIndex].ogTriangles.Length, _meshAssets[currentMeshAssetIndex].trianglesSubMesh[0].value);

        _previewMeshFilter.mesh = GetMeshSubmesh0(_previewMeshFilter.mesh, vertices,uvs,triangles,vertexColors);
    }
    #endregion

}
