using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryController : MonoBehaviour, IConveyorItemGate
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public ItemRecipeAsset[] itemRecipeAssets;
    public ItemRecipeAsset selected;

    public FactoryItemGate[] inputItemGates;
    public FactoryItemGate[] outputItemGates;
    public int[] inputItemCount;
    public int[] outputItemCount;
    #endregion

    #region Functions
    public bool CanReceiveItem(ItemAsset itemAsset,float distanceToEnd)
    {
        for (int i = 0; i < selected.input.Length; i++)
        {
            if(itemAsset == selected.input[i].item && inputItemCount[i] <= selected.input[i].item.maxStackCount)
            {
                return true;
            }
        }
        return false;
    }
    #endregion



    #region Methods
    void Start()
    {
        SetupGates();
        SetRecipe(itemRecipeAssets[0]);
    }
   void Update()
    {
        
    }
    void SetupGates()
    {
        for (int i = 0; i < inputItemGates.Length; i++)
        {
            inputItemGates[i].Setup(this,ConveyorConnectionData.ConveyorSide.Input);
        }
        for (int i = 0; i < outputItemGates.Length; i++)
        {
            outputItemGates[i].Setup(this, ConveyorConnectionData.ConveyorSide.Output);
        }
       
    }
    public void SetRecipe(ItemRecipeAsset itemRecipeAsset)
    {
        selected = itemRecipeAsset;
        inputItemCount = new int[itemRecipeAsset.input.Length];
        outputItemCount = new int[itemRecipeAsset.output.Length];
    }

    public void ReceiveItem(ItemAsset itemAsset,Transform itemTransform)
    {
        for (int i = 0; i < selected.input.Length; i++)
        {
            if (itemAsset == selected.input[i].item)
            {
              inputItemCount[i]++;
            }
        }
        Destroy(itemTransform.gameObject);
    }

    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
    {

    }
    #endregion

}
