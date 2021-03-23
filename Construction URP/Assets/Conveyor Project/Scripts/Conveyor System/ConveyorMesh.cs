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
    MeshAsset GetSelectedMeshAsset()
    {
        if (_isConveyorSelected.value)
        {         
            return _conveyorMeshAsset;
        }
        else if (_isPipelineSelected.value)
        {
            return _pipelineMeshAsset;
        }
        return null;
    }
    public Mesh GetMesh(Mesh newMesh, Vector3[] vertices, Vector2[] uvs, int[] triangles, Color32[] vertexColors)
    {

        newMesh.Clear();
        newMesh.SetVertices(vertices);
        newMesh.SetUVs(0, uvs);
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
    (Vector3[] vertices,Vector2[] uvs, Color32[] vertexColors) GetVertexDataArrays(int edgeLoopCount, OrientedPoint orientedPoints, EdgeLoop edgeLoop, bool reversedUV = false, MeshAsset.UvMapOrientation uvMapOrientation = MeshAsset.UvMapOrientation.ForwardX, byte newConveyorSpeed = 0, float compansateForwardStretch = 1)
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
                int currentVertexDataArrayIndex = currentLoopArrayIndex + currentVertexData;

                vertices[currentVertexDataArrayIndex] = GetVertex(currentEdgeLoop, edgeLoop.vertexDatas[currentVertexData], orientedPoints);
                uvs[currentVertexDataArrayIndex] = GetUV(currentEdgeLoop, edgeLoop.vertexDatas[currentVertexData], reversedUV, orientedPoints, uvMapOrientation, compansateForwardStretch);
                vertexColors[currentVertexDataArrayIndex] = GetVertexColor(edgeLoop.vertexDatas[currentVertexData], newConveyorSpeed);
            }
        }

        return (vertices, uvs, vertexColors);
    }
    Vector3 GetVertex(int currentEdgeLoop, VertexData vertexData, OrientedPoint orientedPoints)
    {
        return orientedPoints.GetPointInLocalSpace(vertexData.vertexPosition,currentEdgeLoop);
    }
    Vector2 GetUV(int currentEdgeLoop, VertexData vertexData, bool reversedUV, OrientedPoint orientedPoints, MeshAsset.UvMapOrientation uvMapOrientation, float compansateForwardStretch)
    {
        float forwardLength = 0;
        float sidewardLength = 0;
        Vector2 uv = new Vector2();

        if (uvMapOrientation == MeshAsset.UvMapOrientation.ForwardX)
        {         
            forwardLength = vertexData.vertexUV.x > 0 ?orientedPoints.segmentDistanceBackward[currentEdgeLoop]: orientedPoints.segmentDistanceForward[currentEdgeLoop];
            if (IsVertexScrollable(vertexData.vertexColor))
            {
                forwardLength *= compansateForwardStretch;
            }
            sidewardLength = vertexData.vertexUV.y;
            uv = new Vector2(forwardLength, sidewardLength);
        }
        else
        {
            forwardLength = vertexData.vertexUV.y > 0 ? orientedPoints.segmentDistanceBackward[currentEdgeLoop] : orientedPoints.segmentDistanceForward[currentEdgeLoop];
            if (IsVertexScrollable(vertexData.vertexColor))
            {
                forwardLength *= compansateForwardStretch;
            }
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
    public void MeshUpdate(bool reversedUV)
    {
        if (!_drawMesh) return;
        if (_orientedPoints.positions.Length == 0)
        {
            Debug.Log("nima");
            return;
        }
        int targetEdgeLoopCount = _orientedPoints.positions.Length;
        MeshAsset selectedMeshAsset = GetSelectedMeshAsset();
        var vertexDataArrays = GetVertexDataArrays(targetEdgeLoopCount, _orientedPoints, selectedMeshAsset.edgeLoop, reversedUV, selectedMeshAsset.uvMapOrientation, conveyorSpeed, selectedMeshAsset.compensateForwardStretch);
        int[] precomputedTriangles = GetTrimmedPrecomputedTriangles(targetEdgeLoopCount, selectedMeshAsset.edgeLoop, selectedMeshAsset.precomputedTriangles);
        _previewMeshFilter.mesh = GetMesh(_previewMeshFilter.mesh, vertexDataArrays.vertices, vertexDataArrays.uvs, precomputedTriangles, vertexDataArrays.vertexColors);
    }
    public void BakeCollider()
    {
        int targetEdgeLoopCount = _orientedPoints.positions.Length;
        MeshAsset selectedMeshAsset = GetSelectedMeshAsset();
        var vertexDataArrays = GetVertexDataArrays(targetEdgeLoopCount, _orientedPoints, selectedMeshAsset.edgeLoopCollider);
        int[] precomputedTriangles = GetTrimmedPrecomputedTriangles(targetEdgeLoopCount, selectedMeshAsset.edgeLoopCollider, selectedMeshAsset.precomputedTrianglesCollider);
        _previewMeshCollider.sharedMesh = GetMeshCollider(_previewMeshCollider.sharedMesh,vertexDataArrays.vertices,precomputedTriangles);
    }
    #endregion

}
