using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item Asset", menuName = "ScriptableObjects/ItemAsset", order = 5)]
public class ItemAsset : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackCount;
    public GameObject prefab;
}
