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
        meshAsset.trianglesSubMesh = GetTrianglesSubmesh(meshAsset);
        meshAsset.segmentUV = GetSegmentUVs(meshAsset);
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
    public int loop1;
    public int loop2;
    public int ogVert1;
    public int ogVert2;
    public int equalsX;
    public int equalsY;
    public int notEqualsIndex;
    public int segmentRepeat;
    public int addSegment;
    public List<Vector3> test;
    NestedArrayInt[] GetSegmentUV(MeshAsset meshAsset)
    {
        List<NestedArrayInt> uvSegment = new List<NestedArrayInt>();

        for (int currentMatchLoopFirst = 0; currentMatchLoopFirst < _triangles.Length; currentMatchLoopFirst++)
        {
            loop1++;
            for (int currentMatchLoopSecond = 0; currentMatchLoopSecond < _triangles.Length; currentMatchLoopSecond++)
            {
                loop2++;
                if (meshAsset.ogVertices[_triangles[currentMatchLoopFirst]].z < 0)
                {
                    ogVert1++;
                    if (meshAsset.ogVertices[_triangles[currentMatchLoopSecond]].z > 0)
                    {
                        ogVert2++;
                        //if (meshAsset.ogVertices[_triangles[currentMatchLoopFirst]].x == meshAsset.ogVertices[_triangles[currentMatchLoopSecond]].x)
                        if (Mathf.Approximately(meshAsset.ogVertices[_triangles[currentMatchLoopFirst]].x, meshAsset.ogVertices[_triangles[currentMatchLoopSecond]].x))
                        {
                            equalsX++;
                            //if (meshAsset.ogVertices[_triangles[currentMatchLoopFirst]].y == meshAsset.ogVertices[_triangles[currentMatchLoopSecond]].y)
                            if (Mathf.Approximately(meshAsset.ogVertices[_triangles[currentMatchLoopFirst]].y, meshAsset.ogVertices[_triangles[currentMatchLoopSecond]].y))
                            {

                                equalsY++;
                                if (_triangles[currentMatchLoopFirst] != _triangles[currentMatchLoopSecond])
                                {
                                    notEqualsIndex++;
                                    NestedArrayInt segment = new NestedArrayInt
                                    {
                                        value = new int[] { _triangles[currentMatchLoopFirst], _triangles[currentMatchLoopSecond] }
                                    };
                                    bool repeat = false;

                                    for (int i = 0; i < uvSegment.Count; i++)
                                    {
                                        if (segment.value[0] == uvSegment[i].value[0] && segment.value[1] == uvSegment[i].value[1])
                                        {
                                            repeat = true;
                                            segmentRepeat++;
                                        }
                                    }
                                    if (repeat == false)
                                    {
                                        uvSegment.Add(segment);
                                        addSegment++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        meshAsset.segmentUV = uvSegment.ToArray();

        return uvSegment.ToArray();
    }
   
    NestedArrayInt[] GetSegmentUVs(MeshAsset meshAsset)
    {
        List<NestedArrayInt> uvSegment = new List<NestedArrayInt>();
        for (int currentTriangle = 2; currentTriangle < 9; currentTriangle += 3)
        {
            List<int> allVertices = new List<int>();
            allVertices.Add(_triangles[currentTriangle]);
            allVertices.Add(_triangles[currentTriangle - 1]);
            allVertices.Add(_triangles[currentTriangle - 2]);

           List<int> sameAxisVertices = new List<int>();


            for (int firstLoop = 0; firstLoop < allVertices.Count; firstLoop++)
            {
                loop1++;
                for (int secondLoop = 0; secondLoop < allVertices.Count; secondLoop++)
                {
                    loop2++;
                    if (secondLoop != firstLoop)
                    {
                        notEqualsIndex++;
                        if (Mathf.Approximately(meshAsset.ogVertices[allVertices[firstLoop]].x, meshAsset.ogVertices[allVertices[secondLoop]].x) && Mathf.Approximately(meshAsset.ogVertices[allVertices[firstLoop]].y, meshAsset.ogVertices[allVertices[secondLoop]].y))
                        {
                            equalsX++;
                            equalsY++;
                            sameAxisVertices.Add(allVertices[secondLoop]);

                        }
                    
                    }

                }
            }
            if (sameAxisVertices.Count != 0)
            {
               
            }
            else
            {
              
            }
            /*
            NestedArrayInt segment;
            if (meshAsset.ogVertices[sameAxisVertices[0]].z <= 0)
            {
                 segment = new NestedArrayInt
                {
                    value = new int[] {sameAxisVertices[0], sameAxisVertices[1]}
                };
            }
            else
            {
                 segment = new NestedArrayInt
                {
                    value = new int[] { sameAxisVertices[1], sameAxisVertices[0] }
                };
            }
            if (!uvSegment.Contains(segment))
            {
                uvSegment.Add(segment);
                addSegment++;
            }
            */
        }    
        return uvSegment.ToArray();
    }
}