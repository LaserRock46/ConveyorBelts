using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour, IConveyorItemGate
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    public ItemAsset testItem;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private bool _isStartOccupied;
    private bool _isEndOccupied;
    [SerializeField] private bool _isDirectionReversed;
  
    private IConveyorItemGate _consecutiveFactoryOrConveyor;  

    private List<int> _itemType;
    public ItemTransmission itemTransmission = new ItemTransmission();
    private float _itemHalfwayLength = 0.5f;

    #endregion

    #region Functions
    public bool CanReceiveItem(ItemAsset itemAsset)
    {
        if(itemTransmission.itemsProgress.Count > 0)
        {
            if (!_isDirectionReversed)
            {
                return itemTransmission.itemsProgress[itemTransmission.itemsProgress.Count - 1] - _itemHalfwayLength > _itemHalfwayLength;
            }
            else
            {
                return itemTransmission.itemsProgress[itemTransmission.itemsProgress.Count - 1] + _itemHalfwayLength < itemTransmission.totalDistance - _itemHalfwayLength;
            }
        }
        return true;
    }
    public static Vector3[] PositionsLocalToWorld(Vector3[] local, Transform self)
    {
        Vector3[] world = new Vector3[local.Length];
        for (int i = 0; i < world.Length; i++)
        {
            world[i] = self.TransformPoint(local[i]);
        }
        return world;
    }
    public static Quaternion[] RotationsLocalToWorld(Quaternion[] local,Transform self)
    {
        Quaternion[] world = new Quaternion[local.Length];
        for (int i = 0; i < world.Length; i++)
        {
            world[i] = self.rotation * local[i];
        }
        return world;
    }
    #endregion



    #region Methods
    void Update()
    {
        itemTransmission.Update();
    }
    public void ReceiveItem(ItemAsset itemAsset, Transform itemTransform)
    {
        itemTransmission.AddItem(itemTransform,itemAsset);
    }
    public void Setup(bool isDirectionReversed, OrientedPoint orientedPoints, IConveyorItemGate consecutiveFactoryOrConveyor,float speed)
    {
        bool isNull = consecutiveFactoryOrConveyor != null;
        Debug.Log("setup in CC " + gameObject.name + "consecutive " + isNull + " " + consecutiveFactoryOrConveyor);
        _isDirectionReversed = isDirectionReversed;
        Vector3[] positions = PositionsLocalToWorld(orientedPoints.positions,transform);
        AssignConsecutiveItemGate(consecutiveFactoryOrConveyor);
        itemTransmission.CreatePath(isDirectionReversed,speed,positions,orientedPoints.segmentDistanceForward,orientedPoints.totalDistance,_itemHalfwayLength);
    }
    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
    {
        _consecutiveFactoryOrConveyor = conveyorItemGate;
        itemTransmission.consecutiveFactoryOrConveyor = conveyorItemGate;
        if (conveyorItemGate != null)
        {
            itemTransmission.consecutiveExist = true;
        }
        else
        {
            itemTransmission.consecutiveExist = false;
        }
    }
    [ContextMenu("TestSpawnItem")]
    void TestSpawnItem()
    {
        GameObject test = Instantiate(testItem.prefab);
        itemTransmission.AddItem(test.transform,testItem);
    }
    #endregion

}
