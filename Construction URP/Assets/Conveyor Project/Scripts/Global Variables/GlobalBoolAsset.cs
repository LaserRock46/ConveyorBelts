using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Global Bool", menuName = "ScriptableObjects/GlobalBool", order = 2)]
public class GlobalBoolAsset : ScriptableObject, ISerializationCallbackReceiver
{
    public bool defaultValue;
    public bool value;

    public void SetValue(bool value)
    {
        this.value = value;
    }
    public void ResetToDefault()
    {
        value = defaultValue;
    }
    public void OnAfterDeserialize()
    {
        value = defaultValue;
    }
    public void OnBeforeSerialize() 
    { 
    }
}

