using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuildingAssetResource
{
    public Sprite resourceIcon;
    public int resourceCost;
    public enum ConsumptionType {PerBuilding, PerUnitLength}
    public ConsumptionType consumptionType = ConsumptionType.PerBuilding;
}
