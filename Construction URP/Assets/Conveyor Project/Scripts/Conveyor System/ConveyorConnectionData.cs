using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorSystem
{
    [System.Serializable]
    public class ConveyorConnectionData
    {
        public bool isInitialized;
        public Pillar alignedToPillar;
        public bool isAlignedToExistingPillar;
        public enum PillarSide { Front, Back }
        public PillarSide occupiedPillarSide;
        public enum ConveyorSide { None, Input, Output }
        public ConveyorSide conveyorSide;

        public ConveyorConnectionData(bool isInitialized, Pillar alignedToPillar, bool isAlignedToExistingPillar, PillarSide occupiedPillarSide, ConveyorSide conveyorSide)
        {
            this.isInitialized = isInitialized;
            this.alignedToPillar = alignedToPillar;
            this.isAlignedToExistingPillar = isAlignedToExistingPillar;

            this.occupiedPillarSide = occupiedPillarSide;
            this.conveyorSide = conveyorSide;
        }

        public ConveyorConnectionData(ConveyorSide conveyorSide)
        {
            this.conveyorSide = conveyorSide;
        }
        public void CorrectConveyorSide(ConveyorSide conveyorSide)
        {
            this.conveyorSide = conveyorSide;
        }
    }
}
