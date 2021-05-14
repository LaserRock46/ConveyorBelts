using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ConveyorSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Item Recipe Asset", menuName = "ScriptableObjects/ItemRecipeAsset", order = 4)]

    public class ItemRecipeAsset : ScriptableObject
    {
        public RecipeComponent[] input;
        public RecipeComponent[] output;
        public float productionTime;
    }
    [System.Serializable]
    public class RecipeComponent
    {
        public ItemAsset item;
        public int amount;
    }
}
