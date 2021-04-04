using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour, IConveyorItemGate
{
     #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private bool _isStartOccupied;
    private bool _isEndOccupied;
    [SerializeField] private bool _isDirectionReversed;
  
    private IConveyorItemGate _consecutiveFactoryOrConveyor;

    [SerializeField] private Vector3[] _positions;
    [SerializeField] private Quaternion[] _rotations;
    private List<Transform> _items;
    private List<int> _itemType;
    private List<float> _itemProgress;

    #endregion

    #region Functions
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
    public void PassItem()
    {
     
    }
    public void Setup(bool isDirectionReversed, Vector3[] positions, Quaternion[] rotations)
    {
        _isDirectionReversed = isDirectionReversed;
        _positions = PositionsLocalToWorld(positions,transform);
        _rotations = RotationsLocalToWorld(rotations,transform);
    }
    #endregion

}
