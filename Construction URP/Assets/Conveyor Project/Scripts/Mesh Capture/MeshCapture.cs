using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshCapture
{
    public Mesh mesh;

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
    bool IsOriginalVertexInFront(Vector3 originalVertex)
    {       
        return originalVertex.z >= 0;
    }
    Edge[] GetEdges(VertexData[] vertexDatas, int[] originalTriangles, Vector3[] originalVertices)
    {
        List<Edge> edges = new List<Edge>();
        for (int vertexIndex = 2; vertexIndex < originalTriangles.Length; vertexIndex += 3)
        {        
            int[] triangle = new int[] {originalTriangles[vertexIndex - 2], originalTriangles[vertexIndex - 1], originalTriangles[vertexIndex] };
            Edge.IndiceLoopOrder[] backLoopVertexIndices = GetBackLoopVertexIndices(triangle,originalVertices);
            if (ContainsTwoBackLoops(backLoopVertexIndices))
            {
                Edge edge = GetEdgeWithWindingOrder(vertexDatas, backLoopVertexIndices,triangle,originalVertices);
                edges.Add(edge);
            }
        }
        return edges.ToArray();
    }
    bool ContainsTwoBackLoops(Edge.IndiceLoopOrder[] backLoopVertexIndices)
    {
        int count = 0;
        for (int i = 0; i < backLoopVertexIndices.Length; i++)
        {
            if (backLoopVertexIndices[i] == Edge.IndiceLoopOrder.Back) count++;
        }
        return count == 2;
    }
    Edge.IndiceLoopOrder[] GetBackLoopVertexIndices(int[] triangle, Vector3[] originalVertices)
    {
        Edge.IndiceLoopOrder[] backLoopVertexIndices = new Edge.IndiceLoopOrder[3];
        for (int i = 0; i < triangle.Length; i++)
        {
            int originalVertexIndex = triangle[i];
            backLoopVertexIndices[i] = IsOriginalVertexInFront(originalVertices[originalVertexIndex])? Edge.IndiceLoopOrder.Front:Edge.IndiceLoopOrder.Back;
        }
        return backLoopVertexIndices;
    }
    Edge GetEdgeWithWindingOrder(VertexData[] vertexDatas, Edge.IndiceLoopOrder[] backLoopVertexIndices, int[] triangle, Vector3[] originalVertices)
    {
        Edge.IndiceLoopOrder[] triangleFromBackLoopOrder = new Edge.IndiceLoopOrder[3];
        Edge.IndiceLoopOrder[] triangleFromFrontLoopOrder = new Edge.IndiceLoopOrder[3];
        VertexData[] triangleFromBackLoop = new VertexData[3];
        VertexData[] triangleFromFrontLoop = new VertexData[3];

        triangleFromBackLoopOrder[0] = backLoopVertexIndices[0];
        triangleFromBackLoopOrder[1] = backLoopVertexIndices[1];
        triangleFromBackLoopOrder[2] = backLoopVertexIndices[2];
      
        for (int i = 0; i < triangleFromBackLoop.Length; i++)
        {         
            triangleFromBackLoop[i] = GetMatchingVertexData(vertexDatas,FrontIndiceToBackIndice(triangleFromBackLoopOrder[i],triangle,originalVertices));
        }

        var diagonalIndicesFromQuad = GetDiagonalIndicesFromQuad(backLoopVertexIndices, triangle, originalVertices);
        triangleFromFrontLoopOrder[0] = diagonalIndicesFromQuad.diagonalIndiceLoopOrder[0];
        triangleFromFrontLoopOrder[1] = diagonalIndicesFromQuad.diagonalIndiceLoopOrder[1];
        triangleFromFrontLoopOrder[2] = Edge.IndiceLoopOrder.Front;
        triangleFromFrontLoop[0] = GetMatchingVertexData(vertexDatas,);
        triangleFromFrontLoop[1] = GetMatchingVertexData(vertexDatas, diagonalIndicesFromQuad.diagonalIndices[1]);
        int backLoopDiagonalIndice = diagonalIndicesFromQuad.diagonalIndiceLoopOrder[0] == Edge.IndiceLoopOrder.Back ? diagonalIndicesFromQuad.diagonalIndices[0] : diagonalIndicesFromQuad.diagonalIndices[1];
        triangleFromFrontLoop[2] = GetMatchingVertexData(vertexDatas,backLoopDiagonalIndice);


        return new Edge(triangleFromBackLoopOrder, triangleFromFrontLoopOrder, triangleFromBackLoop, triangleFromFrontLoop);         
    }
    int FrontIndiceToBackIndice(Edge.IndiceLoopOrder indiceLoopOrder, int[] triangle, Vector3[] originalVertices)
    {
        int backIndice = 0;
        for (int i = 0; i < triangle.Length; i++)
        {
            if (indiceLoopOrder == Edge.IndiceLoopOrder.Back)
            {
                backIndice = triangle[i];
            }
            else
            {
                backIndice = GetVertexInSameLineFromTriangle(triangle[i], triangle, originalVertices);
            }
        }
        return backIndice;
    }
    (int[] diagonalIndices, Edge.IndiceLoopOrder[] diagonalIndiceLoopOrder, int backLoopInSameLineIndice) GetDiagonalIndicesFromQuad(Edge.IndiceLoopOrder[] backLoopVertexIndices, int[] triangle, Vector3[] originalVertices)
    {
        List<int> diagonalIndices = new List<int>();
        List<Edge.IndiceLoopOrder> diagonalIndicesLoopOrder = new List<Edge.IndiceLoopOrder>();
        int backLoopInSameLineIndice = 0;

        for (int i = 0; i < triangle.Length; i++)
        {
            if (!IsVertexInSameLineFromTriangle(i, triangle, originalVertices))
            {
                diagonalIndices.Add(triangle[i]);
                diagonalIndicesLoopOrder.Add(backLoopVertexIndices[i]);
            }
            else if (backLoopVertexIndices[i] == Edge.IndiceLoopOrder.Back)
            {
                backLoopInSameLineIndice = triangle[i];
            }
        }
        return (diagonalIndices.ToArray(),diagonalIndicesLoopOrder.ToArray(),backLoopInSameLineIndice);
    }
    bool IsVertexInSameLineFromTriangle(int currentTriangleIndice,int[] triangle, Vector3[] originalVertices)
    {       
        for (int i = 0; i < triangle.Length; i++)
        {
                if (IsOriginalVertexInSameLine(originalVertices[currentTriangleIndice], originalVertices[triangle[i]]) && currentTriangleIndice != triangle[i]) return true;
        }
        return false;
    }
    int GetVertexInSameLineFromTriangle(int currentTriangleIndice, int[] triangle, Vector3[] originalVertices)
    {
        for (int i = 0; i < triangle.Length; i++)
        {
            if (IsOriginalVertexInSameLine(originalVertices[currentTriangleIndice], originalVertices[triangle[i]]) && currentTriangleIndice != triangle[i])
            {
                return triangle[i];
            }
        }
        return 0;
    }
    bool IsOriginalVertexInSameLine(Vector3 a,Vector3 b)
    {
        Vector2 aV2 = a;
        Vector2 bV2 = b;
        return Vector2.Distance(aV2,bV2) < 0.001f;
    }
    VertexData GetMatchingVertexData(VertexData[] vertexDatas, int triangle)
    {
        VertexData vertexData = null;
        for (int i = 0; i < vertexDatas.Length; i++)
        {
            if(triangle == vertexDatas[i].originalVertexIndex)
            {
                vertexData = vertexDatas[i];
            }
        }
        if(vertexData == null)
        {
            Debug.Log("null");
        }
        return vertexData;
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
        int triangleIndiceFromBackLoop0 = edge.triangleFromBackLoop[0].vertexIndex + GetBaseLoopIndex(edge.triangleFromBackLoopOrder[0], baseBackLoop, baseFrontLoop);
        int triangleIndiceFromBackLoop1 = edge.triangleFromBackLoop[1].vertexIndex + GetBaseLoopIndex(edge.triangleFromBackLoopOrder[1], baseBackLoop, baseFrontLoop);
        int triangleIndiceFromBackLoop2 = edge.triangleFromBackLoop[2].vertexIndex + GetBaseLoopIndex(edge.triangleFromBackLoopOrder[2], baseBackLoop, baseFrontLoop);

        int triangleIndiceFromFrontLoop0 = edge.triangleFromFrontLoop[0].vertexIndex + GetBaseLoopIndex(edge.triangleFromBackLoopOrder[0], baseBackLoop, baseFrontLoop);
        int triangleIndiceFromFrontLoop1 = edge.triangleFromFrontLoop[1].vertexIndex + GetBaseLoopIndex(edge.triangleFromBackLoopOrder[1], baseBackLoop, baseFrontLoop);
        int triangleIndiceFromFrontLoop2 = edge.triangleFromFrontLoop[2].vertexIndex + GetBaseLoopIndex(edge.triangleFromBackLoopOrder[2], baseBackLoop, baseFrontLoop);

        int[] quad = new int[6];
        
        quad[0] = triangleIndiceFromBackLoop0;
        quad[1] = triangleIndiceFromBackLoop1;
        quad[2] = triangleIndiceFromBackLoop2;
        quad[3] = triangleIndiceFromFrontLoop0;
        quad[4] = triangleIndiceFromFrontLoop1;
        quad[5] = triangleIndiceFromFrontLoop2;
       
        return quad;
    }
    int GetBaseLoopIndex(Edge.IndiceLoopOrder indiceLoopOrder, int baseBackLoop, int baseFrontLoop)
    {
        if(indiceLoopOrder == Edge.IndiceLoopOrder.Back)
        {
            return baseBackLoop;
        }
        else
        {
            return baseFrontLoop;
        }
    }
}