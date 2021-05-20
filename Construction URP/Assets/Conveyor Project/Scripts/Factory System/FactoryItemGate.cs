using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConveyorSystem;
[System.Serializable]
public class FactoryItemGate
{
  
    public Pillar pillar;

    public void Setup(IConveyorItemGate thisFactoryItemGate, ConveyorConnectionData.ConveyorDirection gateType)
    {
        ConveyorConnectionData.PillarSide pillarSide = gateType == ConveyorConnectionData.ConveyorDirection.Input ? ConveyorConnectionData.PillarSide.Front : ConveyorConnectionData.PillarSide.Back;
        pillar.Setup(gateType,pillarSide,thisFactoryItemGate);
    }
   
}
