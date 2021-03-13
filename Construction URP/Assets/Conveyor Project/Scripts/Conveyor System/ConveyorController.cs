using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
     #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public bool isStartOccupied;
    public bool isEndOccupied;
    public GameObject startConnector;
    public GameObject endConnector;
    #endregion

    #region Functions

    #endregion



    #region Methods
  public void SwapConnectors()
    {
        Vector3 startConnectorPosition = startConnector.transform.position;
        Quaternion startConnectorRotation = startConnector.transform.rotation;
        startConnector.transform.SetPositionAndRotation(endConnector.transform.position,endConnector.transform.rotation);
        endConnector.transform.SetPositionAndRotation(startConnectorPosition,startConnectorRotation);
    }
    #endregion

}
