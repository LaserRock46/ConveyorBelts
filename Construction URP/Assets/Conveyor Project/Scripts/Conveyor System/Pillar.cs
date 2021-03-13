using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private bool _isOccupiedFront = false;
    private bool _isOccupiedBack = false;
    public Transform self;
    public Transform tipAnchor;
    public int indexInPillarStack = 1;
    public List<GameObject> dependsOn = new List<GameObject>();
    #endregion

    #region Functions
    public bool IsOccupiedFront()
    {
        return _isOccupiedFront;
    }
    public bool IsOccupiedBack()
    {
        return _isOccupiedBack;
    }
    #endregion



    #region Methods    
    public void TryDestroy()
    {
        if(dependsOn.Count != 0)
        {
            // Can destroy
        }
        else
        {
            // Can't destroy
        }
    }
    public void OccupyFront()
    {
        _isOccupiedFront = true;       
    }
    #endregion

}
