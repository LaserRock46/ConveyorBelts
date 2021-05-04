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
    public ConveyorSystem.ConveyorConstructor conveyorConstructor;
    public Toggle conveyorConstructorToggle;
    public Toggle[] toggleGroup;
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
    void DisableAnyOtherToggle(Toggle except)
    {
        for (int i = 0; i < toggleGroup.Length; i++)
        {
            if(toggleGroup[i].isOn && toggleGroup[i] != except)
            {
                toggleGroup[i].isOn = false;
            }
        }   
    }
    public void ToggleConveyorConstructor(bool isOn)
    {
        //conveyorConstructor.EnableConveyorConstructor(isOn);
        DisableAnyOtherToggle(conveyorConstructorToggle);
    }
    #endregion

}
