using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTrianhleAxis : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    //[Header("Fields", order = 1)]
    public bool fromAsset;
    public MeshFilter meshFilter;
    public Vector3[] vertices;
    public int[] triangles;
    public MeshAsset meshAsset;
    public int triangleIndex;

    public int line1, line2, line3;
    public float tolerance = 0.01f;
    public bool mode;
    #endregion

    #region Functions

    #endregion



    #region Methods
    void Start()
    {
        triangles = meshFilter.mesh.triangles;
        if (fromAsset)
        {
            vertices = meshAsset.ogVertices;
        }
        else
        {
            vertices = meshFilter.mesh.vertices;
        }
    }
    bool IsAxisEquals(Vector3 a, Vector3 b, float tolerance = 0.01f)
    {
        Vector2 testA = a;
        Vector2 testB = b;
        return Vector2.Distance(a, b) < tolerance;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            triangleIndex -= 3;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            triangleIndex += 3;
        }
        if (mode)
        {
            Manual();
        }
        else
        {
            Auto();
        }

    }
    void Auto()
    {
        for (int i = 2; i < triangles.Length; i += 3)
        {
            int line1 = triangles[i];
            int line2 = triangles[i - 1];
            int line3 = triangles[i - 2];

            if (IsAxisEquals(vertices[line1], vertices[line2]))
            {
                Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line2]), Color.green);
            }
            else
            {
                Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line2]), Color.red);
            }
            if (IsAxisEquals(vertices[line1], vertices[line3]))
            {
                Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line3]), Color.green);
            }
            else
            {
                Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line3]), Color.red);
            }
            if (IsAxisEquals(vertices[line3], vertices[line2]))
            {
                Debug.DrawLine(transform.TransformPoint(vertices[line3]), transform.TransformPoint(vertices[line2]), Color.green);
            }
            else
            {
                Debug.DrawLine(transform.TransformPoint(vertices[line3]), transform.TransformPoint(vertices[line2]), Color.red);
            }
        }
    }
    void Manual()
    {
        line1 = triangles[triangleIndex];
        line2 = triangles[triangleIndex - 1];
        line3 = triangles[triangleIndex - 2];

        if (IsAxisEquals(vertices[line1], vertices[line2]))
        {
            Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line2]), Color.green);
        }
        else
        {
            Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line2]), Color.red);
        }
        if (IsAxisEquals(vertices[line1], vertices[line3]))
        {
            Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line3]), Color.green);
        }
        else
        {
            Debug.DrawLine(transform.TransformPoint(vertices[line1]), transform.TransformPoint(vertices[line3]), Color.red);
        }

        if (IsAxisEquals(vertices[line3], vertices[line2]))
        {
            Debug.DrawLine(transform.TransformPoint(vertices[line3]), transform.TransformPoint(vertices[line2]), Color.green);
        }
        else
        {
            Debug.DrawLine(transform.TransformPoint(vertices[line3]), transform.TransformPoint(vertices[line2]), Color.red);
        }
    }
}
  
    #endregion

