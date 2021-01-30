using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public ConveyorConstructor conveyorConstructor;
    public Toggle conveyorConstructorToggle;
    #endregion

    #region Functions

    #endregion



    #region Methods
    void Start()
    {
        
    }
   void Update()
    {
        UpdateToggles();
    }
    void UpdateToggles()
    {
        if(!conveyorConstructor.isConveyorConstructorEnabled && conveyorConstructorToggle.isOn)
        {
            conveyorConstructorToggle.SetIsOnWithoutNotify(false); 
        }
    }
    public void ToggleConveyorConstructor(bool isOn)
    {
        conveyorConstructor.EnableConveyorConstructor(isOn);
    }
    #endregion

}
