using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCapture : MonoBehaviour
{
    public Mesh mesh;
    public MeshAsset meshAsset;

    private int[] _triangles;
    private List<NestedArrayInt> _trianglesSubMesh = new List<NestedArrayInt>();

    public void GetData()
    {
        if (mesh == null)
        {
            Debug.Log("mesh == null");
            return;
        }
        if (meshAsset == null)
        {
            Debug.Log("meshAsset == null");
            return;
        }
        if(mesh.name != meshAsset.meshName)
        {
            Debug.Log("mesh.name != meshAsset.name");
            return;
        }

        meshAsset.vertices = mesh.vertices;
        meshAsset.uv = mesh.uv;
        _triangles = mesh.triangles;
       
        meshAsset.subMeshCount = mesh.subMeshCount;
        _trianglesSubMesh.Clear();
        for (int i = 0; i < meshAsset.subMeshCount; i++)
        {
            _trianglesSubMesh.Add(new NestedArrayInt());
            _trianglesSubMesh[i].value = mesh.GetTriangles(i);
        }
        meshAsset.trianglesSubMesh = GetTrianglesSubmesh();
        meshAsset.segmentUV = GetSegmentUV();
    }
    NestedArrayInt[] GetTrianglesSubmesh()
    {
        List<NestedArrayInt> trianglesSubmesh = new List<NestedArrayInt>();
        for (int i = 0; i < meshAsset.subMeshCount; i++)
        {
            trianglesSubmesh.Add(new NestedArrayInt());
            trianglesSubmesh[i].value = new int[_trianglesSubMesh[i].value.Length * 128];
        }
        for (int i0 = 0; i0 < 128; i0++)
        {
            for (int i1 = 0; i1 < meshAsset.subMeshCount; i1++)
            {
                int triangleSegmentPositionInArray = _trianglesSubMesh[i1].value.Length * i0;
                for (int i2 = 0; i2 < _trianglesSubMesh[i1].value.Length; i2++)
                {
                    int targetTrianglePositionInTriangleIndexArray = triangleSegmentPositionInArray + i2;

                    int baseValue = 0;
                    int previousMaxValue = 0;
                    if (i0 == 0)
                    {
                        baseValue = _trianglesSubMesh[i1].value[i2];
                    }
                    if (i0 > 0)
                    {
                        baseValue = _trianglesSubMesh[i1].value[i2];

                        previousMaxValue = meshAsset.vertices.Length * i0;
                    }

                    int triangleValue = baseValue + previousMaxValue;                   
                    trianglesSubmesh[i1].value[targetTrianglePositionInTriangleIndexArray] = triangleValue;
                }
            }
        }
        return trianglesSubmesh.ToArray();
    }
    NestedArrayInt[] GetSegmentUV()
    {
        List<NestedArrayInt> uvSegment = new List<NestedArrayInt>();

        for (int i0 = 0; i0 < _triangles.Length; i0++)
        {
            for (int i1 = 0; i1 < _triangles.Length; i1++)
            {
                if (meshAsset.vertices[_triangles[i0]].z < 0)
                {
                    if (meshAsset.vertices[_triangles[i1]].z > 0)
                    {
                        if (meshAsset.vertices[_triangles[i0]].x == meshAsset.vertices[_triangles[i1]].x)
                        {
                            if (meshAsset.vertices[_triangles[i0]].y == meshAsset.vertices[_triangles[i1]].y)
                            {
                                if (_triangles[i0] != _triangles[i1])
                                {
                                    NestedArrayInt segment = new NestedArrayInt
                                    {
                                        value = new int[] { _triangles[i0], _triangles[i1] }
                                    };
                                    bool repeat = false;

                                    for (int i = 0; i < uvSegment.Count; i++)
                                    {
                                        if (segment.value[0] == uvSegment[i].value[0] && segment.value[1] == uvSegment[i].value[1])
                                        {
                                            repeat = true;
                                        }
                                    }
                                    if (repeat == false)
                                    {
                                        uvSegment.Add(segment);
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
}