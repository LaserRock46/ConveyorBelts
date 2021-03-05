using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSimpleQuad : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public MeshFilter meshFilter;
    public Vector3[] vertices = new Vector3[4];
    public int[] triangles = new int[6];
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods
    void Start()
    {
        
    }
   void Update()
    {
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = triangles;
    }
    #endregion

}
