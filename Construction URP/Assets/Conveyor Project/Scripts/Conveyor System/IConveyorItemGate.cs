using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ConveyorSystem
{
    public interface IConveyorItemGate
    {
        public bool CanReceiveItem(ItemAsset itemAsset, float distanceToEnd);
        public ConveyorController GetConveyor();
        public Collider GetCollider();
        public void ReceiveItem(ItemAsset itemAsset, Transform itemTransform);
        public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate);


    }
}
