using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTabHotkeys : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public Toggle[] toggles;
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods
   void Update()
    {
        HotkeyUpdate();
    }
    void HotkeyUpdate()
    {      
        if (Input.GetKeyDown(KeyCode.Alpha1) && toggles[0].IsInteractable())
        {
            toggles[0].isOn = !toggles[0].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && toggles[1].IsInteractable())
        {
            toggles[1].isOn = !toggles[1].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && toggles[2].IsInteractable())
        {
            toggles[2].isOn = !toggles[2].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && toggles[3].IsInteractable())
        {
            toggles[3].isOn = !toggles[3].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && toggles[4].IsInteractable())
        {
            toggles[4].isOn = !toggles[4].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && toggles[5].IsInteractable())
        {
            toggles[5].isOn = !toggles[5].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && toggles[6].IsInteractable())
        {
            toggles[6].isOn = !toggles[6].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && toggles[7].IsInteractable())
        {
            toggles[7].isOn = !toggles[7].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && toggles[8].IsInteractable())
        {
            toggles[8].isOn = !toggles[8].isOn;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0) && toggles[9].IsInteractable())
        {
            toggles[9].isOn = !toggles[9].isOn;
        }
    }
    #endregion

}
