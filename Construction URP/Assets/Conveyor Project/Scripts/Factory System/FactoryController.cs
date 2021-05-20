using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConveyorSystem;
public class FactoryController : MonoBehaviour, IConveyorItemGate
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    bool space;
    bool res;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public ItemRecipeAsset[] itemRecipeAssets;
    public ItemRecipeAsset selected;

    public FactoryItemGate[] inputItemGates;
    public FactoryItemGate[] outputItemGates;
    public int[] inputItemCount;
    public int[] outputItemCount;

    private bool _isWorking = false;

    private IConveyorItemGate _consecutiveConveyor;

    #endregion

    #region Functions
    public ConveyorController GetConveyor()
    {
        return null;
    }
    public Collider GetCollider()
    {
        return gameObject.GetComponent<Collider>();
    }
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
    bool IsEnoughResourcesForProduction()
    {
        int enoughInThisInput = 0;
        for (int i = 0; i < selected.input.Length; i++)
        {
            if(inputItemCount[i] >= selected.input[i].amount)
            {
                enoughInThisInput++;
            }
        }       
        return selected.input.Length == 0 || enoughInThisInput == selected.input.Length;
    }
    bool IsEnoughSpaceInStack()
    {
        int enoughInThisOutput = 0;
        for (int i = 0; i < selected.output.Length; i++)
        {
            if (outputItemCount[i] + selected.output[i].amount <= selected.output[i].item.maxStackCount)
            {
                enoughInThisOutput++;
            }
        }
        return enoughInThisOutput == selected.output.Length;
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
        ProductionUpdate();
        ItemReleaseUpdate();
    }
    void SetupGates()
    {
        for (int i = 0; i < inputItemGates.Length; i++)
        {
            inputItemGates[i].Setup(this,ConveyorConnectionData.ConveyorDirection.Input);
        }
        for (int i = 0; i < outputItemGates.Length; i++)
        {
            outputItemGates[i].Setup(this, ConveyorConnectionData.ConveyorDirection.Output);
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
    void ProductionUpdate()
    {
        if (_isWorking == false)
        {
            if (IsEnoughSpaceInStack() && IsEnoughResourcesForProduction())
            {
                _isWorking = true;
                StartCoroutine(ItemProduction(selected.productionTime));
            }
        }
    }
    private IEnumerator ItemProduction(float productionTime)
    {
        yield return new WaitForSeconds(productionTime);
        ItemProductionFinished();
    }
    void ItemProductionFinished()
    {
        for (int i = 0; i < inputItemCount.Length; i++)
        {
            inputItemCount[i] -= selected.input[i].amount;
        }
        for (int i = 0; i < outputItemCount.Length; i++)
        {
            outputItemCount[i] += selected.output[i].amount;
        }
        _isWorking = false;
    }
    void ItemReleaseUpdate()
    {
        if (_consecutiveConveyor != null)
        {
            for (int i = 0; i < outputItemCount.Length; i++)
            {
                if (outputItemCount[i] > 0 && _consecutiveConveyor.CanReceiveItem(selected.output[i].item, 0))
                {;
                    outputItemCount[i]--;
                    GameObject newItem = Instantiate(selected.output[i].item.prefab);
                    _consecutiveConveyor.ReceiveItem(selected.output[i].item, newItem.transform);
                }
            }
        }
    }
    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
    {
        _consecutiveConveyor = conveyorItemGate;
    }
    #endregion

}
