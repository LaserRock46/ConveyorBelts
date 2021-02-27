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


        meshAsset.ogVertices = mesh.vertices;
        meshAsset.ogTriangles = mesh.triangles;
        meshAsset.ogUvs = mesh.uv;
        meshAsset.ogVertexColors = mesh.colors32;
        _triangles = mesh.triangles;

        meshAsset.subMeshCount = mesh.subMeshCount;
        _trianglesSubMesh.Clear();
        for (int i = 0; i < meshAsset.subMeshCount; i++)
        {
            _trianglesSubMesh.Add(new NestedArrayInt());
            _trianglesSubMesh[i].value = mesh.GetTriangles(i);
        }
        meshAsset.trianglesSubMesh = GetTrianglesSubmeshAlternative(meshAsset);
        meshAsset.uvSegments = GetSegmentUVs(meshAsset);

        /// refactored
        Vector3[] originalVertices = mesh.vertices;
        Vector2[] originalUVs = mesh.uv;
        int[] originalTriangles = mesh.triangles;
        Color32[] originalVertexColors = mesh.colors32;

        VertexData[] vertexDatas = GetVertexDatas(originalVertices,originalUVs,originalVertexColors);
        Edge[] edges = GetEdges(vertexDatas, originalTriangles, originalVertices);
        EdgeLoop edgeLoop = new EdgeLoop(vertexDatas,edges);
        int[] precomputedTriangles = GetPrecomputedTrianglesArray(meshAsset.loopCount,edgeLoop);
    }
    NestedArrayInt[] GetTrianglesSubmesh(MeshAsset meshAsset)
    {
        List<NestedArrayInt> trianglesSubmesh = new List<NestedArrayInt>();
        for (int i = 0; i < meshAsset.subMeshCount; i++)
        {
            trianglesSubmesh.Add(new NestedArrayInt());
            trianglesSubmesh[i].value = new int[_trianglesSubMesh[i].value.Length * meshAsset.loopCount];
        }

        for (int currentEdgeLoop = 0; currentEdgeLoop < meshAsset.loopCount; currentEdgeLoop++)
        {
            for (int currentSubmesh = 0; currentSubmesh < meshAsset.subMeshCount; currentSubmesh++)
            {
                int triangleSegmentPositionInArray = _trianglesSubMesh[currentSubmesh].value.Length * currentEdgeLoop;
                for (int i2 = 0; i2 < _trianglesSubMesh[currentSubmesh].value.Length; i2++)
                {
                    int targetTrianglePositionInTriangleIndexArray = triangleSegmentPositionInArray + i2;

                    int baseValue = 0;
                    int previousMaxValue = 0;
                    if (currentEdgeLoop == 0)
                    {
                        baseValue = _trianglesSubMesh[currentSubmesh].value[i2];
                    }
                    if (currentEdgeLoop > 0)
                    {
                        baseValue = _trianglesSubMesh[currentSubmesh].value[i2];

                        previousMaxValue = meshAsset.ogVertices.Length * currentEdgeLoop;
                    }

                    int triangleValue = baseValue + previousMaxValue;
                    trianglesSubmesh[currentSubmesh].value[targetTrianglePositionInTriangleIndexArray] = triangleValue;
                }
            }
        }
        return trianglesSubmesh.ToArray();
    }
   
    bool IsOriginalVertexInFront(Vector3 originalVertex)
    {
        if (originalVertex.z >= 0)
        {
            return true;
        }
        return false;
    }
    NestedArrayInt[] GetTrianglesSubmeshAlternative(MeshAsset meshAsset)
    {
        List<NestedArrayInt> trianglesSubmesh = new List<NestedArrayInt>();
        for (int i = 0; i < meshAsset.subMeshCount; i++)
        {
            trianglesSubmesh.Add(new NestedArrayInt());
            trianglesSubmesh[i].value = new int[(_trianglesSubMesh[i].value.Length/2) * meshAsset.loopCount];
        }

        for (int currentEdgeLoop = 0; currentEdgeLoop < meshAsset.loopCount - 1; currentEdgeLoop++)
        {
            for (int currentSubmesh = 0; currentSubmesh < meshAsset.subMeshCount; currentSubmesh++)
            {
                int trianglesHalfLength = _trianglesSubMesh[currentSubmesh].value.Length / 2;
                int triangleSegmentPositionInArray = trianglesHalfLength * currentEdgeLoop;
                for (int currentOgTriangle = 0; currentOgTriangle < _trianglesSubMesh[currentSubmesh].value.Length; currentOgTriangle++)
                {
                    int targetTrianglePositionInTriangleIndexArray = triangleSegmentPositionInArray + currentOgTriangle;
                  
                   
                    int baseValue = _trianglesSubMesh[currentSubmesh].value[currentOgTriangle];

                    int halfVerticesLength = meshAsset.ogVertices.Length / 2;
                    int previousMaxValue = halfVerticesLength * currentEdgeLoop;
                    int nextMaxValue = halfVerticesLength * (currentEdgeLoop + 1);

                    int triangleValue = 0;

                    if (currentEdgeLoop > 1)
                    {
                        if (IsOriginalVertexInFront(meshAsset.ogVertices[baseValue]))
                        {
                            triangleValue = baseValue + nextMaxValue;
                        }
                        else
                        {
                            triangleValue = baseValue + previousMaxValue;
                        }
                    }
                    else
                    {
                        triangleValue = baseValue;
                    }
                    triangleValue = baseValue;
                    trianglesSubmesh[currentSubmesh].value[targetTrianglePositionInTriangleIndexArray] = triangleValue;
                }
            }
        }
        return trianglesSubmesh.ToArray();
    }
  
    bool IsAxisEquals(Vector3 a, Vector3 b, float tolerance = 0.01f)
    {
        Vector2 testA = a;
        Vector2 testB = b;
        return Vector2.Distance(a, b) < tolerance;
    }
    UvSegment[] GetSegmentUVs(MeshAsset meshAsset)
    {
        List<UvSegment> uvSegment = new List<UvSegment>();
        for (int currentTriangle = 2; currentTriangle < _triangles.Length; currentTriangle += 3)
        {
            List<int> allVertices = new List<int>();
            allVertices.Add(_triangles[currentTriangle]);
            allVertices.Add(_triangles[currentTriangle - 1]);
            allVertices.Add(_triangles[currentTriangle - 2]);

           List<int> sameAxisVertices = new List<int>();


            for (int firstLoop = 0; firstLoop < allVertices.Count; firstLoop++)
            {             
                for (int secondLoop = 0; secondLoop < allVertices.Count; secondLoop++)
                {
                    if (secondLoop != firstLoop)
                    {          
                        if(IsAxisEquals(meshAsset.ogVertices[allVertices[firstLoop]], meshAsset.ogVertices[allVertices[secondLoop]]))
                        {
                            sameAxisVertices.Add(allVertices[secondLoop]);
                        }                 
                    }
                }
            }        
            
            UvSegment segment;
            if (meshAsset.ogVertices[sameAxisVertices[0]].z <= 0)
            {
                bool reverseDirection = meshAsset.ogUvs[sameAxisVertices[0]].x >= 0;
                segment = new UvSegment(sameAxisVertices[0], sameAxisVertices[1], 0, reverseDirection);          
            }
            else
            {
                bool reverseDirection = meshAsset.ogUvs[sameAxisVertices[1]].x >= 0;
                segment = new UvSegment(sameAxisVertices[1], sameAxisVertices[0], 0, reverseDirection);        
            }
            if (!uvSegment.Contains(segment))
            {
                uvSegment.Add(segment);
            }
            
        }    
        return uvSegment.ToArray();
    }
    //// refectored system
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
            int[] backLoopVertexIndices = GetBackLoopVertexIndices(vertexIndex,originalTriangles,originalVertices);
            Edge edge = GetEdgeWithWindingOrder(vertexDatas, backLoopVertexIndices);
            edges.Add(edge);
        }
        return edges.ToArray();
    }
    int[] GetBackLoopVertexIndices(int vertexIndex,int[] originalTriangles, Vector3[] originalVertices)
    {
        List<int> backLoopVertexIndices = new List<int>();
        for (int currentVertexIndex = vertexIndex; currentVertexIndex >= vertexIndex - 3; currentVertexIndex--)
        {
            int originalVertexIndex = originalTriangles[currentVertexIndex];
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
                vertexDataRight = vertexDatas[currentVertexData];
            }
            if (backLoopVertexIndices[1] == vertexDatas[currentVertexData].originalVertexIndex)
            {
                vertexDataLeft = vertexDatas[currentVertexData];
            }
        }
        Edge edge = new Edge(vertexDataRight,vertexDataLeft);

        return edge;
    }   
    int[] GetPrecomputedTrianglesArray(int loopCount, EdgeLoop edgeLoop)
    {
        int trianglesLength = loopCount * edgeLoop.vertexDatas.Length;
        int[] triangles = new int[trianglesLength];
       

        for (int currentLoop = 0; currentLoop < loopCount; currentLoop++)
        {          
            for (int currentEdge = 0; currentEdge < edgeLoop.edges.Length; currentEdge++)
            {
                int[] getQuadFromEdge = GetQuadFromEdge(edgeLoop.edges[currentEdge],currentLoop,edgeLoop.vertexDatas.Length);
            }
        }

        return triangles;
    }
    int[] GetQuadFromEdge(Edge edge, int loopIndex, int loopVertexDatasLength)
    {
        int[] quad = new int[6];
        int baseBackLoop = loopVertexDatasLength * loopIndex;
        int baseFrontLoop = loopVertexDatasLength * (loopIndex + 1);

        int triangleIndiceFromBackLoop0 = baseBackLoop;
        int triangleIndiceFromBackLoop1 = baseBackLoop;
        int triangleIndiceFromBackLoop2 = baseFrontLoop;

        int triangleIndiceFromFrontLoop0 = baseFrontLoop;
        int triangleIndiceFromFrontLoop1 = baseFrontLoop;
        int triangleIndiceFromFrontLoop2 = baseBackLoop;

        quad[0] = triangleIndiceFromBackLoop0;
        quad[1] = triangleIndiceFromBackLoop1;
        quad[2] = triangleIndiceFromBackLoop2;
        quad[3] = triangleIndiceFromFrontLoop0;
        quad[4] = triangleIndiceFromFrontLoop1;
        quad[5] = triangleIndiceFromFrontLoop2;

        return quad;
    }
}