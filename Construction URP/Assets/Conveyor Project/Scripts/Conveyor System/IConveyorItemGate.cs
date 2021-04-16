using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConveyorItemGate
{
    public bool CanReceiveItem(ItemAsset itemAsset);
    public void ReceiveItem(ItemAsset itemAsset, Transform itemTransform);
    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate);
}
