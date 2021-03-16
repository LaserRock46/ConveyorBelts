using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "New Building Asset", menuName = "ScriptableObjects/BuildingAsset", order = 3)]
public class BuildingAsset: ScriptableObject
{
    public string buildingName;
    public GlobalBoolAsset[] globalBools;
    public Sprite buildingIcon;
    public BuildingAssetResource[] buildingAssetResources;

    public void SetValues(bool value)
    {
        foreach (GlobalBoolAsset globalBool in globalBools)
        {
            globalBool.SetValue(value);
        }
    }

}
