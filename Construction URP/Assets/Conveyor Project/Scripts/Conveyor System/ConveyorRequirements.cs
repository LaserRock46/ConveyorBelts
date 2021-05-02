using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorRequirements
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    //[Header("Fields", order = 1)]
    #endregion

    #region Functions

    #endregion



    #region Methods   
    public void Test(out RequirementsTestResult requirementsTestResult)
    {
        requirementsTestResult = new RequirementsTestResult();
    }
    #endregion

}
public class RequirementsTestResult
{
    public bool meetAllRequirements;
    public bool isTooShort;
    public bool isTooLong;
    public bool isTooSharp;
    public bool isTooStep;
    public bool isOverlappingOthers;
    public bool isInvalidShape;
    public bool directionNotMatch;
    public bool notEnaughResources;
}
