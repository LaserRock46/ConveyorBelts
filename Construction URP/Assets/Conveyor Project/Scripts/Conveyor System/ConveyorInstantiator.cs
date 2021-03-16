using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorInstantiator : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    [SerializeField] private Transform _buildingsRoot;
    [SerializeField] private GameObject _conveyorPrefab;
    [SerializeField] private GameObject _pipelinePrefab;
    #endregion

    #region Functions

    #endregion



    #region Methods
    public void InstantiateInGameplayMode()
    {

    }
    public void InstantiateInSaveLoadMode()
    {

    }
    #endregion

}
