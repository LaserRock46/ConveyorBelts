using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ConveyorConnectionData
{
    public Pillar allignedToPillar;
    public bool isAllignedToExistingPillar;
    public bool isAllignedExistingConveyorReversed;
    public enum PillarSide {Front,Back }
    public PillarSide occupiedPillarSide;
    public enum ConveyorSide { Start,End}
    public ConveyorSide conveyorSide;

    public ConveyorConnectionData(Pillar allignedToPillar, bool isAllignedToExistingPillar, bool isAllignedExistingConveyorReversed, PillarSide occupiedPillarSide, ConveyorSide conveyorSide)
    {
        this.allignedToPillar = allignedToPillar;
        this.isAllignedToExistingPillar = isAllignedToExistingPillar;
        this.isAllignedExistingConveyorReversed = isAllignedExistingConveyorReversed;
        this.occupiedPillarSide = occupiedPillarSide;
        this.conveyorSide = conveyorSide;
    }
}
