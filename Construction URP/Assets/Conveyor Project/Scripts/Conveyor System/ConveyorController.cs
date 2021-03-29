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

    private Vector3[] _positions;
    private Quaternion[] _rotations;
    private List<Transform> _items;
    private List<int> _itemType;
    private List<float> _itemProgress;

    #endregion

    #region Functions

    #endregion



    #region Methods
    public void PassItem()
    {
     
    }
    public void Setup(bool isDirectionReversed, Vector3[] positions, Quaternion[] rotations)
    {
        _isDirectionReversed = isDirectionReversed;
        _positions = positions;
        _rotations = rotations;
    }
    #endregion

}
