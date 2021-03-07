using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshCapture
{
    public Mesh mesh;
    //public MeshAsset meshAsset;

    private int[] _triangles;
    private List<NestedArrayInt> _trianglesSubMesh = new List<NestedArrayInt>();

    public void GetData(MeshAsset meshAsset)
    {
        if (mesh == null)
        {
            Debug.LogError("mesh == null");
            return;
        }
        if (meshAsset == null)
        {
            Debug.LogError("meshAsset == null");
            return;
        }

        Vector3[] originalVertices = mesh.vertices;
        Vector2[] originalUVs = mesh.uv;
        int[] originalTriangles = mesh.triangles;
        Color32[] originalVertexColors = mesh.colors32;

        VertexData[] vertexDatas = GetVertexDatas(originalVertices,originalUVs,originalVertexColors);
        Edge[] edges = GetEdges(vertexDatas, originalTriangles, originalVertices);
        EdgeLoop edgeLoop = new EdgeLoop(vertexDatas,edges);
        int[] precomputedTriangles = GetPrecomputedTrianglesArray(meshAsset.loopCount,edgeLoop);

        meshAsset.edgeLoop = edgeLoop;
        meshAsset.precomputedTriangles = precomputedTriangles;
    } 
    bool IsOriginalVertexInFront(Vector3 originalVertex)
    {
        if (originalVertex.z >= 0)
        {
            return true;
        }
        return false;
    }
   
    bool IsAxisEquals(Vector3 a, Vector3 b, float tolerance = 0.01f)
    {    
        return Vector2.Distance(a, b) < tolerance;
    }
   
    VertexData[] GetVertexDatas(Vector3[] originalVertices,Vector2[] originalUVs, Color32[] originalVertexColors)
    {
        List<VertexData> vertexDatas = new List<VertexData>();
        for (int currentVertex = 0; currentVertex < originalVertices.Length; currentVertex++)
        {
            if (!IsOriginalVertexInFront(originalVertices[currentVertex]))
            {
                int originalVertexIndex = currentVertex;
                int vertexIndex = vertexDatas.Count;
                Vector3 vertexPosition = originalVertices[currentVertex];
                Vector2 vertexUV = originalUVs[currentVertex];
                Color32 vertexColor = originalVertexColors[currentVertex];
                VertexData vertexData = new VertexData(originalVertexIndex, vertexIndex, vertexPosition, vertexUV, vertexColor);
                vertexDatas.Add(vertexData);
            }
        }
        return vertexDatas.ToArray();
    }
    Edge[] GetEdges(VertexData[] vertexDatas, int[] originalTriangles, Vector3[] originalVertices)
    {
        List<Edge> edges = new List<Edge>();
        for (int vertexIndex = 2; vertexIndex < originalTriangles.Length; vertexIndex += 3)
        {        
            int[] originalTriangle = new int[] { originalTriangles[vertexIndex - 2], originalTriangles[vertexIndex - 1], originalTriangles[vertexIndex] };
            int[] backLoopVertexIndices = GetBackLoopVertexIndices(originalTriangle,originalVertices);
            if (backLoopVertexIndices.Length == 2)
            {
                Edge edge = GetEdgeWithWindingOrder(vertexDatas, backLoopVertexIndices);
                edges.Add(edge);
            }
        }
        return edges.ToArray();
    }
    int[] GetBackLoopVertexIndices(int[] originalTriangle, Vector3[] originalVertices)
    {   
        List<int> backLoopVertexIndices = new List<int>();
        for (int currentVertexIndex = 0; currentVertexIndex < originalTriangle.Length; currentVertexIndex++)
        {
            int originalVertexIndex = originalTriangle[currentVertexIndex];
            if (!IsOriginalVertexInFront(originalVertices[originalVertexIndex]))
            {
                backLoopVertexIndices.Add(originalVertexIndex);
            }
        }                  
        return backLoopVertexIndices.ToArray();
    }
    Edge GetEdgeWithWindingOrder(VertexData[] vertexDatas, int[] backLoopVertexIndices)
    {
        VertexData vertexDataRight = null;
        VertexData vertexDataLeft = null; 

        for (int currentVertexData = 0; currentVertexData < vertexDatas.Length; currentVertexData++)
        {
            if(backLoopVertexIndices[0] == vertexDatas[currentVertexData].originalVertexIndex)
            {
                vertexDataLeft = vertexDatas[currentVertexData];
            }
            if (backLoopVertexIndices[1] == vertexDatas[currentVertexData].originalVertexIndex)
            {
                vertexDataRight = vertexDatas[currentVertexData];
            }
        }
        Edge edge = new Edge(vertexDataRight,vertexDataLeft);

        return edge;
    }
    int[] GetPrecomputedTrianglesArray(int loopCount, EdgeLoop edgeLoop)
    {
        List<int> precomputedTriangles = new List<int>();       

        for (int currentLoop = 0; currentLoop < loopCount - 1; currentLoop++)
        {          
            for (int currentEdge = 0; currentEdge < edgeLoop.edges.Length; currentEdge++)
            {
                int[] quad = GetQuadFromEdge(edgeLoop.edges[currentEdge],currentLoop,edgeLoop.vertexDatas.Length);
                precomputedTriangles.AddRange(quad);
            }
        }

        return precomputedTriangles.ToArray();
    }
    int[] GetQuadFromEdge(Edge edge, int loopIndex, int loopVertexDatasLength)
    {
        int baseBackLoop = loopVertexDatasLength * loopIndex;
        int baseFrontLoop = loopVertexDatasLength * (loopIndex + 1);

        int triangleIndiceFromBackLoop0 = baseBackLoop + edge.vertexDataRight.vertexIndex;
        int triangleIndiceFromBackLoop1 = baseBackLoop + edge.vertexDataLeft.vertexIndex; 
        int triangleIndiceFromBackLoop2 = baseFrontLoop + edge.vertexDataLeft.vertexIndex; 

        int triangleIndiceFromFrontLoop0 = baseFrontLoop + edge.vertexDataLeft.vertexIndex; 
        int triangleIndiceFromFrontLoop1 = baseFrontLoop + edge.vertexDataRight.vertexIndex; 
        int triangleIndiceFromFrontLoop2 = baseBackLoop + edge.vertexDataRight.vertexIndex; 

        int[] quad = new int[6];

        quad[0] = triangleIndiceFromBackLoop0;
        quad[1] = triangleIndiceFromBackLoop1;
        quad[2] = triangleIndiceFromBackLoop2;
        quad[3] = triangleIndiceFromFrontLoop0;
        quad[4] = triangleIndiceFromFrontLoop1;
        quad[5] = triangleIndiceFromFrontLoop2;

        return quad;
    }
}