using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawWireMesh : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public Mesh selfMesh;
    public Transform self;
    public Color color= Color.green; 
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods
    void Start()
    {
        selfMesh = GetComponent<MeshCollider>().sharedMesh;
        self = transform;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireMesh(selfMesh, self.position, self.rotation);
    }
    #endregion

}
