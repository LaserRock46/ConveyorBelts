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
    [SerializeField] private bool _isOccupiedFront = false;
    [SerializeField] private bool _isOccupiedBack = false;
    public IConveyorItemGate frontSideConveyor;
    public IConveyorItemGate backSideConveyor;
    public ConveyorConnectionData.ConveyorSide conveyorSide;
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
    public void Setup(ConveyorConnectionData.ConveyorSide conveyorSide, ConveyorConnectionData.PillarSide occupiedPillarSide)
    {
        this.conveyorSide = conveyorSide;
        if(occupiedPillarSide == ConveyorConnectionData.PillarSide.Front)
        {
            _isOccupiedFront = true;
        }
        if (occupiedPillarSide == ConveyorConnectionData.PillarSide.Back)
        {
            _isOccupiedBack = true;
        }
    }
    #endregion

}
