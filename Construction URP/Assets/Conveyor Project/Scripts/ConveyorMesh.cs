using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorMesh
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private Bezier _bezier;
    [SerializeField] private MeshFilter _previewMeshFilter;

    
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods
    public void AssignBezier(Bezier bezier)
    {
        _bezier = bezier;
    }
    public void MeshUpdate(bool reverse)
    {
     
    }
    #endregion

}
