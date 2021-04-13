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
    public void Setup(ConveyorConnectionData.ConveyorSide conveyorSide, ConveyorConnectionData.PillarSide occupiedPillarSide, IConveyorItemGate conveyorItemGate)
    {
        this.conveyorSide = conveyorSide;
        if(occupiedPillarSide == ConveyorConnectionData.PillarSide.Front)
        {
            _isOccupiedFront = true;
            frontSideConveyor = conveyorItemGate;

            if(backSideConveyor != null && conveyorSide == ConveyorConnectionData.ConveyorSide.Input)
            {
                backSideConveyor.AssignConsecutiveItemGate(conveyorItemGate);
            }
        }
        if (occupiedPillarSide == ConveyorConnectionData.PillarSide.Back)
        {
            _isOccupiedBack = true;
            backSideConveyor = conveyorItemGate;

            if (frontSideConveyor != null && conveyorSide == ConveyorConnectionData.ConveyorSide.Input)
            {
                frontSideConveyor.AssignConsecutiveItemGate(conveyorItemGate);
            }
        }
    }
    #endregion

}
