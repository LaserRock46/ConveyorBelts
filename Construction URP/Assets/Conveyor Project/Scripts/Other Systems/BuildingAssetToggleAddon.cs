using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingAssetToggleAddon : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public BuildingAsset buildingAsset;
    public Toggle toggle;
    public ToggleGroup toggleGroup;
    public ResourceTooltipController resourceTooltipController;
    public Image buildingIcon;
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods 
    void Start()
    {
        
    }
   void Update()
    {
        
    }
    private void OnValidate()
    {
        if (!buildingAsset || !buildingIcon || !buildingAsset.buildingIcon)
            return;
        AssignIcon();
    }
    void AssignIcon()
    {

        buildingIcon.sprite = buildingAsset.buildingIcon;
        buildingIcon.enabled = true;
        toggle.interactable = true;
    }
    #endregion

}
