using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryController : MonoBehaviour, IConveyorItemGate
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public ItemRecipeAsset[] itemRecipeAssets;
    public ItemRecipeAsset selected;

    public Pillar[] inputs;
    public Pillar output;
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods
    void Start()
    {
        SetupInputsAndOutputs();
    }
   void Update()
    {
        
    }
    void SetupInputsAndOutputs()
    {

    }

    public void PassItem()
    {
       
    }

    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
    {

    }
    #endregion

}
