using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorSystem
{
    public class Pillar : MonoBehaviour
    {
        #region Temp
        //[Header("Temporary Things", order = 0)]
        #endregion

        #region Fields
        [Header("Fields", order = 1)]
        public IConveyorItemGate frontSideConveyor;
        public IConveyorItemGate backSideConveyor;
        public ConveyorConnectionData.ConveyorSide conveyorSide;
        public ConveyorConnectionData.ConveyorSide frontConveyorSide;
        public ConveyorConnectionData.ConveyorSide backConveyorSide;
        public Transform self;
        public Transform tipAnchor;
        public int indexInPillarStack = 1;
        public List<GameObject> dependsOn = new List<GameObject>();
        #endregion

        #region Functions
        public bool IsOccupiedFront()
        {
            return frontSideConveyor != null;
        }
        public bool IsOccupiedBack()
        {
            return backSideConveyor != null;
        }
        #endregion



        #region Methods  

        public void TryDestroy()
        {
            if (dependsOn.Count != 0)
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
            if (occupiedPillarSide == ConveyorConnectionData.PillarSide.Front)
            {
                frontSideConveyor = conveyorItemGate;
                if (backSideConveyor != null && conveyorSide == ConveyorConnectionData.ConveyorSide.Input)
                {
                    backSideConveyor.AssignConsecutiveItemGate(conveyorItemGate);
                }
            }
            if (occupiedPillarSide == ConveyorConnectionData.PillarSide.Back)
            {
                backSideConveyor = conveyorItemGate;
                if (frontSideConveyor != null && conveyorSide == ConveyorConnectionData.ConveyorSide.Input)
                {
                    frontSideConveyor.AssignConsecutiveItemGate(conveyorItemGate);
                }
            }
        }
        [ContextMenu("Remove Gate Front")]
        void RemoveItemGateFront()
        {
            frontSideConveyor = null;
        }
        [ContextMenu("Remove Gate Back")]
        void RemoveItemGateBack()
        {
            backSideConveyor = null;
        }
        #endregion

    }
}
