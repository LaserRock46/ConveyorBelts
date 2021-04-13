using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConveyorItemGate
{
    public void PassItem();
    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate);
}
