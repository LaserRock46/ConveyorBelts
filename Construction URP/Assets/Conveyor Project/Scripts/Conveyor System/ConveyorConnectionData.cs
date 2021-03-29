using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorConnectionData
{
    public Pillar alignedToPillar;
    public bool isAlignedToExistingPillar;
    public enum PillarSide {Front,Back }
    public PillarSide occupiedPillarSide;
    public enum ConveyorSide {None, Start, End}
    public ConveyorSide conveyorSide;

    public ConveyorConnectionData(Pillar alignedToPillar, bool isAlignedToExistingPillar, PillarSide occupiedPillarSide, ConveyorSide conveyorSide)
    {
        this.alignedToPillar = alignedToPillar;
        this.isAlignedToExistingPillar = isAlignedToExistingPillar;
  
        this.occupiedPillarSide = occupiedPillarSide;
        this.conveyorSide = conveyorSide;
    }

    public ConveyorConnectionData(ConveyorSide conveyorSide)
    {
        this.conveyorSide = conveyorSide;
    }
}
