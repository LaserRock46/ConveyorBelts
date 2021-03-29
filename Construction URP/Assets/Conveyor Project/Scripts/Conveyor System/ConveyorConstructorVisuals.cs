using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorConstructorVisuals : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    [SerializeField] private Transform _arrowStart;
    [SerializeField] private Transform _arrowEnd;
    [SerializeField] private Vector3 _arrowScale = new Vector3(0.2f,0.5f,0.2f);
    #endregion

    #region Functions

    #endregion



    #region Methods
    public void UpdateArrowsDirection(bool isConveyorDirectionReversed)
    {
        if (isConveyorDirectionReversed)
        {
            _arrowStart.localScale = -_arrowScale;
            _arrowEnd.localScale = -_arrowScale;
        }
        else
        {
            _arrowStart.localScale = _arrowScale;
            _arrowEnd.localScale = _arrowScale;
        }
    }
    #endregion

}
