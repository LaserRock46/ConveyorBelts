using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorMesh
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    [SerializeField] private bool _drawMesh;
    public Vector2[] uvs3;

    public bool reversedUV;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    [SerializeField] private MeshAsset _conveyorMeshAsset;
    [SerializeField] private MeshAsset _pipelineMeshAsset;
    [SerializeField] private GlobalBoolAsset _isConveyorSelected;
    [SerializeField] private GlobalBoolAsset _isPipelineSelected;

    private OrientedPoint _orientedPoints;
    [SerializeField] private Transform _previewTransform;
    [SerializeField] private MeshFilter _previewMeshFilter;
    [SerializeField] private MeshRenderer _previewMeshRenderer;
    [SerializeField] private MeshCollider _previewMeshCollider;
    [Range(0,255)] public byte conveyorSpeed = 50;
    #endregion

    #region Functions
    public Mesh GetMesh(Mesh newMesh, Vector3[] vertices, Vector2[] uvs, Vector2[] uvs3, int[] triangles, Color32[] vertexColors)
    {

        newMesh.Clear();
        newMesh.SetVertices(vertices);
        newMesh.SetUVs(0, uvs);
        newMesh.SetUVs(3, uvs3);      
        newMesh.SetTriangles(triangles, 0);
        newMesh.SetColors(vertexColors);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
     
        return newMesh;
    }
    public Mesh GetMeshCollider(Mesh newMesh, Vector3[] vertices,int[] triangles)
    {
        if (!newMesh)
        {
            newMesh = new Mesh();
        }
        else
        {
            newMesh.Clear();
        }
        newMesh.SetVertices(vertices);     
        newMesh.SetTriangles(triangles, 0);          
        newMesh.RecalculateNormals();   

        return newMesh;
    }
    int[] GetTrimmedPrecomputedTriangles(int edgeLoopCount, EdgeLoop edgeLoop, int[] precomputedTriangles)
    {
        int trianglesLength = (edgeLoopCount - 1) * edgeLoop.edges.Length * 6;
        int[] trimmedPrecomputedTriangles = new int[trianglesLength];

        for (int i = 0; i < trimmedPrecomputedTriangles.Length; i++)
        {
            trimmedPrecomputedTriangles[i] = precomputedTriangles[i];
        }
        return trimmedPrecomputedTriangles;
    }
    (Vector3[] vertices,Vector2[] uvs, Vector2[] uvs3, Color32[] vertexColors) GetVertexDataArrays(int edgeLoopCount, OrientedPoint orientedPoints, EdgeLoop edgeLoop, bool reversedUV = false, MeshAsset.UvMapOrientation uvMapOrientation = MeshAsset.UvMapOrientation.ForwardX, byte newConveyorSpeed = 0, float compansateForwardStretch = 1)
    {
        int newVertexDataArrayLength = edgeLoopCount * edgeLoop.vertexDatas.Length;
        Vector3[] vertices = new Vector3[newVertexDataArrayLength];
        Vector2[] uvs = new Vector2[newVertexDataArrayLength];
        Vector2[] uvs3 = new Vector2[newVertexDataArrayLength];
        Color32[] vertexColors = new Color32[newVertexDataArrayLength];
      
        for (int currentEdgeLoop = 0; currentEdgeLoop < edgeLoopCount; currentEdgeLoop++)
        {
            int currentLoopArrayIndex = currentEdgeLoop * edgeLoop.vertexDatas.Length;

            for (int currentVertexData = 0; currentVertexData < edgeLoop.vertexDatas.Length; currentVertexData++)
            {
                int currentVertexDataArrayIndex = currentLoopArrayIndex + currentVertexData;

                vertices[currentVertexDataArrayIndex] = GetVertex(currentEdgeLoop, edgeLoop.vertexDatas[currentVertexData], orientedPoints);
                uvs[currentVertexDataArrayIndex] = GetUV(currentEdgeLoop, edgeLoop.vertexDatas[currentVertexData], reversedUV, orientedPoints, uvMapOrientation, compansateForwardStretch);
                uvs3[currentVertexDataArrayIndex] = GetUV3ForRevealEffect(currentEdgeLoop, reversedUV, orientedPoints);
                vertexColors[currentVertexDataArrayIndex] = GetVertexColor(edgeLoop.vertexDatas[currentVertexData], newConveyorSpeed);
              
            }
        }     
        return (vertices, uvs,uvs3, vertexColors);
    }
    Vector3 GetVertex(int currentEdgeLoop, VertexData vertexData, OrientedPoint orientedPoints)
    {
        return orientedPoints.GetPointInLocalSpace(vertexData.vertexPosition,currentEdgeLoop);
    }
    Vector2 GetUV(int currentEdgeLoop, VertexData vertexData, bool reversedUV, OrientedPoint orientedPoints, MeshAsset.UvMapOrientation uvMapOrientation, float compansateForwardStretch)
    {
        float verticalLength = 0;
        float horizontalLength = 0;
        Vector2 uv = new Vector2();

        if (uvMapOrientation == MeshAsset.UvMapOrientation.ForwardX)
        {                 
            verticalLength = GetLengthByUV(orientedPoints.segmentDistanceBackward[currentEdgeLoop], orientedPoints.segmentDistanceForward[currentEdgeLoop], vertexData.vertexUV.x,reversedUV);
            if (IsVertexScrollable(vertexData.vertexColor))
            {
                verticalLength *= compansateForwardStretch;
            }
            horizontalLength = vertexData.vertexUV.y;
            uv = new Vector2(verticalLength, horizontalLength);
        }
        else
        {         
            verticalLength = GetLengthByUV(orientedPoints.segmentDistanceBackward[currentEdgeLoop], orientedPoints.segmentDistanceForward[currentEdgeLoop], vertexData.vertexUV.y, reversedUV);
            if (IsVertexScrollable(vertexData.vertexColor))
            {
                verticalLength *= compansateForwardStretch;
            }
            horizontalLength = vertexData.vertexUV.x;
            uv = new Vector2(horizontalLength, verticalLength);
        }
        return uv;
    }
    Vector2 GetUV3ForRevealEffect(int currentEdgeLoop , bool reversedUV, OrientedPoint orientedPoints)
    {
        float length = reversedUV? orientedPoints.segmentDistanceBackward[currentEdgeLoop] : orientedPoints.segmentDistanceForward[currentEdgeLoop];
        Vector2 uv3 = new Vector2(length, 0);
        return uv3;
    }
    float GetLengthByUV(float backward,float forward,float verticalUVcomponent,bool reversedUV)
    {
        if (reversedUV)
        {
            return verticalUVcomponent > 0.5 ? forward : backward;
        }
        return verticalUVcomponent > 0.5 ? backward : forward;
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
    bool IsVertexScrollable(Color32 vertexColor)
    {
        return vertexColor.g != 0;
    }
    #endregion

    #region Methods
    public void AssignOrientedPoints(OrientedPoint orientedPoints)
    {
        _orientedPoints = orientedPoints;
    }
    public void MeshUpdate(bool reversedUV, ConveyorAsset conveyorAsset)
    {
        if (!_drawMesh) return;
        this.reversedUV = reversedUV;
        if (_orientedPoints.positions.Length == 0)
        {
            Debug.Log("nima");
            return;
        }
        int targetEdgeLoopCount = _orientedPoints.positions.Length;
        MeshAsset selectedMeshAsset = conveyorAsset.conveyorMeshAsset;
        var vertexDataArrays = GetVertexDataArrays(targetEdgeLoopCount, _orientedPoints, selectedMeshAsset.edgeLoop, reversedUV, selectedMeshAsset.uvMapOrientation, conveyorAsset.conveyorSpeed, selectedMeshAsset.compensateForwardStretch);
        int[] precomputedTriangles = GetTrimmedPrecomputedTriangles(targetEdgeLoopCount, selectedMeshAsset.edgeLoop, selectedMeshAsset.precomputedTriangles);
        uvs3 = vertexDataArrays.uvs3;
        _previewMeshFilter.mesh = GetMesh(_previewMeshFilter.mesh, vertexDataArrays.vertices, vertexDataArrays.uvs, vertexDataArrays.uvs3, precomputedTriangles, vertexDataArrays.vertexColors);
    }
    public void BakeCollider(ConveyorAsset conveyorAsset)
    {
        int targetEdgeLoopCount = _orientedPoints.positions.Length;
        MeshAsset selectedMeshAsset = conveyorAsset.conveyorMeshAsset;
        var vertexDataArrays = GetVertexDataArrays(targetEdgeLoopCount, _orientedPoints, selectedMeshAsset.edgeLoopCollider);
        int[] precomputedTriangles = GetTrimmedPrecomputedTriangles(targetEdgeLoopCount, selectedMeshAsset.edgeLoopCollider, selectedMeshAsset.precomputedTrianglesCollider);
        _previewMeshCollider.sharedMesh = GetMeshCollider(_previewMeshCollider.sharedMesh,vertexDataArrays.vertices,precomputedTriangles);
    }
    #endregion

}
