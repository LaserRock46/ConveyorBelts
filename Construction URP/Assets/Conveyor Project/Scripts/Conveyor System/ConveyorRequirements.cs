using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ConveyorSystem
{
    [System.Serializable]
    public class ConveyorRequirements
    {
        #region Temp
        //[Header("Temporary Things", order = 0)]
        #endregion

        #region Fields
        [Header("Fields", order = 1)]
        [SerializeField] private float _minLength;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _minMaxSteepness;
        [SerializeField] private Vector3 _checkBoxSize;
        [SerializeField] private Vector3 _planeIntersectionSize;

        #endregion

        #region Functions
        bool TestForAllRequirements(bool isConnectionDataNotInitialized, bool isTooShort, bool isTooLong, bool isOverlappingOthers, bool isInvalidShape, bool directionNotMatch, bool isNotEnoughResources)
        {
            return isConnectionDataNotInitialized == false && isTooLong == false && isTooShort == false && isOverlappingOthers == false && isInvalidShape == false && directionNotMatch == false && isNotEnoughResources == false;
        }
        bool TestForConnectionDataInitialization(ConveyorConnectionData connectionDataStart, ConveyorConnectionData connectionDataEnd)
        {
            return connectionDataStart.isInitialized == false && connectionDataEnd.isInitialized == false;
        }
        bool TestForMinLength(float totalDistance)
        {
            return totalDistance < _minLength;
        }
        bool TestForMaxLength(float totalDistance)
        {
            return totalDistance > _maxLength;
        }
        bool TestForSteepnes(Quaternion[] rotations)
        {
            for (int i = 0; i < rotations.Length; i++)
            {
                Quaternion flatRotation = Quaternion.Euler(0, rotations[i].eulerAngles.y, rotations[i].eulerAngles.z);
                if (Quaternion.Angle(rotations[i], flatRotation) > _minMaxSteepness)
                {
                    return true;
                }
            }
            return false;
        }
        bool TestForInvalidShape()
        {
            return false;
        }
        bool TestForDirectionOfAttachedConveyor()
        {
            return false;
        }
        bool TestForNotEnoughResources()
        {
            return false;
        }
        #endregion



        #region Methods   
        public void Test(out RequirementsTestResult requirementsTestResult, OrientedPoint orientedPoints, ConveyorConnectionData connectionDataStart, ConveyorConnectionData connectionDataEnd)
        {
            requirementsTestResult = new RequirementsTestResult();
            
        }
        #endregion

    }
    public class RequirementsTestResult
    {
        public bool meetAllRequirements;
        public bool isConnectionDataNotInitialized;
        public bool isTooShort;
        public bool isTooLong;
        public bool isTooSteep;
        public bool isOverlappingOthers;
        public bool isInvalidShape;
        public bool directionNotMatch;
        public bool isNotEnoughResources;

        public RequirementsTestResult()
        {
        }
        public RequirementsTestResult(bool meetAllRequirements, bool isConnectionDataNotInitialized, bool isTooShort, bool isTooLong, bool isTooSteep, bool isOverlappingOthers, bool isInvalidShape, bool directionNotMatch, bool isNotEnoughResources)
        {
            this.meetAllRequirements = meetAllRequirements;
            this.isConnectionDataNotInitialized = isConnectionDataNotInitialized;
            this.isTooShort = isTooShort;
            this.isTooLong = isTooLong;
            this.isTooSteep = isTooSteep;
            this.isOverlappingOthers = isOverlappingOthers;
            this.isInvalidShape = isInvalidShape;
            this.directionNotMatch = directionNotMatch;
            this.isNotEnoughResources = isNotEnoughResources;
        }
    }
}
