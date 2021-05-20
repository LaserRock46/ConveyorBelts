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
        public Collider alignedToConveyorCollider;
        public bool isAlignedToExistingPillar;
        public enum PillarSide { Front, Back }
        public PillarSide occupiedPillarSide;
        public enum ConveyorDirection { None, Input, Output }
        public ConveyorDirection conveyorSide;

        public ConveyorConnectionData(bool isInitialized, Pillar alignedToPillar, bool isAlignedToExistingPillar, PillarSide occupiedPillarSide, ConveyorDirection conveyorSide)
        {
            this.isInitialized = isInitialized;
            this.alignedToPillar = alignedToPillar;
            this.isAlignedToExistingPillar = isAlignedToExistingPillar;

            this.occupiedPillarSide = occupiedPillarSide;
            this.conveyorSide = conveyorSide;
        }

        public ConveyorConnectionData(ConveyorDirection conveyorSide)
        {
            this.conveyorSide = conveyorSide;
        }
        public void CorrectConveyorSide(ConveyorDirection conveyorSide)
        {
            this.conveyorSide = conveyorSide;
        }
    }
}
