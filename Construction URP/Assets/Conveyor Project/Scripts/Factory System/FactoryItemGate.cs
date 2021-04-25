using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactoryItemGate
{
  
    public Pillar pillar;

    public void Setup(IConveyorItemGate thisFactoryItemGate, ConveyorConnectionData.ConveyorSide gateType)
    {
        ConveyorConnectionData.PillarSide pillarSide = gateType == ConveyorConnectionData.ConveyorSide.Input ? ConveyorConnectionData.PillarSide.Front : ConveyorConnectionData.PillarSide.Back;
        pillar.Setup(gateType,pillarSide,thisFactoryItemGate);
    }
   
}