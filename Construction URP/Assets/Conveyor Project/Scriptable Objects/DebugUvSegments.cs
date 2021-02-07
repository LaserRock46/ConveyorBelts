using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUvSegments : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    //[Header("Fields", order = 1)]
    public MeshAsset meshAsset;
    public int segmentIndex;
    public Vector3 startV;
    public Vector3 endV;
    public int start;
    public int end;
    #endregion

    #region Functions

    #endregion



    #region Methods
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            segmentIndex--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            segmentIndex++;
        }
        segmentIndex = Mathf.Clamp(segmentIndex, 0, meshAsset.segmentUV.Length - 1);
        start = meshAsset.segmentUV[segmentIndex].value[0];
        end = meshAsset.segmentUV[segmentIndex].value[1];
        Debug.DrawLine(transform.TransformPoint(meshAsset.ogVertices[start]), transform.TransformPoint(meshAsset.ogVertices[end]), Color.red);
        startV = meshAsset.ogVertices[start];
        endV = meshAsset.ogVertices[end];
    }
    #endregion

}
