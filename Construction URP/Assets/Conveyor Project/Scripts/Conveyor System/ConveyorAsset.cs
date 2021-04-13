using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Conveyor Asset", menuName = "ScriptableObjects/ConveyorAsset")]
public class ConveyorAsset: ScriptableObject
{
    public GlobalBoolAsset isSelected;
    public MeshAsset conveyorMeshAsset;
    public byte conveyorSpeed;
    public GameObject conveyorPrefab;
    public GameObject pillarPrefab;
    public Vector3 pillarHeight;
    public Vector3 pillarToGroundOffset;
    public Vector3 conveyorTipToPillarOffset;

}
