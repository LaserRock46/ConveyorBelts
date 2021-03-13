using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableToggle : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public ToggleGroup toggleGroup;
    #endregion

    #region Functions

    #endregion



    #region Methods 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && toggleGroup.AnyTogglesOn())
        {
            toggleGroup.SetAllTogglesOff();
        }
    }
    #endregion

}
