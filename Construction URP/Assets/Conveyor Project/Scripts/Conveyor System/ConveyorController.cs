using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour, IConveyorItemGate
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    public ItemAsset testItem;
    public float distBetween;
    public float distToEnd;
    public float distToStart;
    public bool canReceive;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private bool _isStartOccupied;
    private bool _isEndOccupied;
    [SerializeField] private bool _isDirectionReversed;
  
    private IConveyorItemGate _consecutiveFactoryOrConveyor;  

    private List<int> _itemType;
    public ItemTransmission itemTransmission = new ItemTransmission();
    [SerializeField] private float _itemHalfwayLength = 0.6f;

    #endregion

    #region Functions
    public bool CanReceiveItem(ItemAsset itemAsset,float distanceToEnd, ref float dist)
    {
        if(itemTransmission.itemsProgress.Count > 0)
        {
            float distanceBetweenThisLastAndStart = 0;
            if (!_isDirectionReversed)
            {               
                distanceBetweenThisLastAndStart = itemTransmission.itemsProgress[itemTransmission.itemsProgress.Count - 1];
            }
            else
            {              
                distanceBetweenThisLastAndStart = itemTransmission.totalDistance - (itemTransmission.totalDistance - itemTransmission.itemsProgress[itemTransmission.itemsProgress.Count - 1]);
            }
            distBetween = distanceBetweenThisLastAndStart + distanceToEnd;
            this.distToEnd = distanceToEnd;
            distToStart = distanceBetweenThisLastAndStart;
            canReceive = distanceBetweenThisLastAndStart - distanceToEnd >= _itemHalfwayLength;
          
            return distanceBetweenThisLastAndStart + distanceToEnd >= _itemHalfwayLength;
        }
        canReceive = true;
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
    public void TestSpawnItem()
    {
        GameObject test = Instantiate(testItem.prefab);
        itemTransmission.AddItem(test.transform,testItem);
    }
    #endregion

}
