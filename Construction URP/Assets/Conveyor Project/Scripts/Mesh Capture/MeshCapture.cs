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
                Edge edge = GetEdgeWithWindingOrder(vertexDatas, backLoopVertexIndices,originalTriangle,originalVertices);
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
    Edge GetEdgeWithWindingOrder(VertexData[] vertexDatas, int[] backLoopVertexIndices,int[] originalTriangle, Vector3[] originalVertices)
    {
        VertexData vertexDataRight = GetMatchingVertexData(backLoopVertexIndices[0],vertexDatas);
        VertexData vertexDataLeft = GetMatchingVertexData(backLoopVertexIndices[1], vertexDatas);

        Vector3 sampleLeftBack = vertexDataLeft.vertexPosition;
        Vector3 sampleRightBack = vertexDataRight.vertexPosition;
        Vector3 sampleLeftFront = vertexDataLeft.vertexPosition;
        sampleLeftFront.z = 1;
        Vector3 sampleRightFront = vertexDataRight.vertexPosition;
        sampleRightFront.z = 1;

        Edge.WindingOrder sampleLeftBackOrder = Edge.WindingOrder.LeftBack;
        Edge.WindingOrder sampleRightBackOrder = Edge.WindingOrder.RigthBack;
        Edge.WindingOrder sampleLeftFrontOrder = Edge.WindingOrder.LeftFront;
        Edge.WindingOrder sampleRightFrontOrder = Edge.WindingOrder.RightFront;

        Vector3 originalQuadNormal = GetQuadNormal(originalVertices[originalTriangle[0]], originalVertices[originalTriangle[1]], originalVertices[originalTriangle[2]]);

        WindingOrderSolution[] solutionsBackLoopTriangle = new WindingOrderSolution[]
        {
            new WindingOrderSolution(sampleLeftBack,sampleRightBack,sampleLeftFront,sampleLeftBackOrder,sampleRightBackOrder,sampleLeftFrontOrder),
            new WindingOrderSolution(sampleRightBack,sampleLeftBack,sampleLeftFront,sampleRightBackOrder,sampleLeftBackOrder,sampleLeftFrontOrder)
        };
        WindingOrderSolution[] solutionsFrontLoopTriangle = new WindingOrderSolution[]
       {
            new WindingOrderSolution(sampleLeftFront,sampleRightFront,sampleRightBack,sampleLeftFrontOrder,sampleRightFrontOrder,sampleRightBackOrder),
            new WindingOrderSolution(sampleRightFront,sampleLeftFront,sampleRightBack,sampleRightFrontOrder,sampleLeftFrontOrder,sampleRightBackOrder)
       };

        Edge.WindingOrder[] fromFrontLoopTriangle = GetTriangleWindingOrderSolution(originalQuadNormal, solutionsFrontLoopTriangle);
        Edge.WindingOrder[] fromBackLoopTriangle = GetTriangleWindingOrderSolution(originalQuadNormal, solutionsBackLoopTriangle);
       
        return new Edge(vertexDataRight, vertexDataLeft, fromFrontLoopTriangle, fromBackLoopTriangle);
    }
    VertexData GetMatchingVertexData(int backLoopVertexIndex,VertexData[] vertexDatas)
    {
        for (int currentVertexData = 0; currentVertexData < vertexDatas.Length; currentVertexData++)
        {
            if (backLoopVertexIndex == vertexDatas[currentVertexData].originalVertexIndex)
            {
                return vertexDatas[currentVertexData];
            }          
        }
        return null;
    }
    Vector3 GetQuadNormal(Vector3 v0,Vector3 v1, Vector3 v2)
    {
        return new Plane(v0,v1,v2).normal;
    }
    Edge.WindingOrder[] GetTriangleWindingOrderSolution(Vector3 originalQuadNormal, WindingOrderSolution[] windingOrderSolutions)
    {
        for (int i = 0; i < windingOrderSolutions.Length; i++)
        {
            Vector3 solutionQuadNormal = GetQuadNormal(windingOrderSolutions[i].v0, windingOrderSolutions[i].v1, windingOrderSolutions[i].v2);
            if (solutionQuadNormal == originalQuadNormal)
            {
                return new Edge.WindingOrder[] { windingOrderSolutions[i].order0, windingOrderSolutions[i].order1, windingOrderSolutions[i].order2 };
            }
        }
        return null;
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

        int triangleIndiceFromBackLoop0 = GetIndice(baseBackLoop, baseFrontLoop, edge.vertexDataLeft, edge.vertexDataRight, edge.fromBackLoopTriangle[0]);
        int triangleIndiceFromBackLoop1 = GetIndice(baseBackLoop, baseFrontLoop, edge.vertexDataLeft, edge.vertexDataRight, edge.fromBackLoopTriangle[1]);
        int triangleIndiceFromBackLoop2 = GetIndice(baseBackLoop, baseFrontLoop, edge.vertexDataLeft, edge.vertexDataRight, edge.fromBackLoopTriangle[2]);

        int triangleIndiceFromFrontLoop0 = GetIndice(baseBackLoop, baseFrontLoop, edge.vertexDataLeft, edge.vertexDataRight, edge.fromFrontLoopTriangle[0]);
        int triangleIndiceFromFrontLoop1 = GetIndice(baseBackLoop, baseFrontLoop, edge.vertexDataLeft, edge.vertexDataRight, edge.fromFrontLoopTriangle[1]);
        int triangleIndiceFromFrontLoop2 = GetIndice(baseBackLoop, baseFrontLoop, edge.vertexDataLeft, edge.vertexDataRight, edge.fromFrontLoopTriangle[2]);


        int[] quad = new int[6];
        quad[0] = triangleIndiceFromBackLoop0;
        quad[1] = triangleIndiceFromBackLoop1;
        quad[2] = triangleIndiceFromBackLoop2;
        quad[3] = triangleIndiceFromFrontLoop0;
        quad[4] = triangleIndiceFromFrontLoop1;
        quad[5] = triangleIndiceFromFrontLoop2;

        return quad;
    }
    int GetIndice(int baseBackLoop, int baseFrontLoop,VertexData vertexDataLeft, VertexData vertexDataRight, Edge.WindingOrder windingOrder)
    {
        switch (windingOrder)
        {
            case Edge.WindingOrder.LeftBack:
                return baseBackLoop + vertexDataLeft.vertexIndex;
            case Edge.WindingOrder.RigthBack:
                return baseBackLoop + vertexDataRight.vertexIndex;
            case Edge.WindingOrder.LeftFront:
                return baseFrontLoop + vertexDataLeft.vertexIndex;
            case Edge.WindingOrder.RightFront:
                return baseFrontLoop + vertexDataRight.vertexIndex;
        }
        return 0;
    }
}