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
        public IConveyorItemGate frontConveyor;
        public IConveyorItemGate backConveyor;
        public ConveyorConnectionData.ConveyorDirection conveyorSide;
        public ConveyorConnectionData.ConveyorDirection frontConveyorSide;
        public ConveyorConnectionData.ConveyorDirection backConveyorSide;
        public Transform self;
        public Transform tipAnchor;
        public int indexInPillarStack = 1;
        public List<GameObject> dependsOn = new List<GameObject>();
        #endregion

        #region Functions
        public bool IsOccupiedFront()
        {
            return frontConveyor != null;
        }
        public bool IsOccupiedBack()
        {
            return backConveyor != null;
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
        public void Setup(ConveyorConnectionData.ConveyorDirection conveyorSide, ConveyorConnectionData.PillarSide occupiedPillarSide, IConveyorItemGate conveyorItemGate)
        {
            this.conveyorSide = conveyorSide;
            if (occupiedPillarSide == ConveyorConnectionData.PillarSide.Front)
            {
                frontConveyor = conveyorItemGate;
                if (backConveyor != null && conveyorSide == ConveyorConnectionData.ConveyorDirection.Input)
                {
                    backConveyor.AssignConsecutiveItemGate(conveyorItemGate);
                }
            }
            if (occupiedPillarSide == ConveyorConnectionData.PillarSide.Back)
            {
                backConveyor = conveyorItemGate;
                if (frontConveyor != null && conveyorSide == ConveyorConnectionData.ConveyorDirection.Input)
                {
                    frontConveyor.AssignConsecutiveItemGate(conveyorItemGate);
                }
            }
        }
        [ContextMenu("Remove Gate Front")]
        void RemoveItemGateFront()
        {
            frontConveyor = null;
        }
        [ContextMenu("Remove Gate Back")]
        void RemoveItemGateBack()
        {
            backConveyor = null;
        }
        #endregion

    }
}
