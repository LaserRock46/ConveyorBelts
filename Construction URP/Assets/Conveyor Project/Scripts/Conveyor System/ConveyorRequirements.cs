using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace ConveyorSystem
{
    [System.Serializable]
    public class ConveyorRequirements
    {
        #region Temp
        [Header("Temporary Things", order = 0)]
        public bool debugOverlapping;
        public bool debugIntersection;
        public bool debugSteepness;

        public List<Vector3> overlappingCenter = new List<Vector3>();
        public List<Vector3> overlappingExtents = new List<Vector3>();
        public List<Quaternion> overlappingOrientation = new List<Quaternion>();

        public List<Vector3> segmentIntersectionCurrent = new List<Vector3>();
        public List<Vector3> segmentIntersectionNextA = new List<Vector3>();
        public List<Vector3> segmentIntersectionNextB = new List<Vector3>();

        public List<Vector3> tooStepPoints = new List<Vector3>();
        #endregion

        #region Fields
        [Header("Fields", order = 1)]
        [SerializeField] private Transform _previewTransform;
        [SerializeField] private Transform _conveyorStartTransform = null;
        [SerializeField] private Transform _conveyorEndTransform = null;
        [SerializeField] private float _minLength;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _minMaxSteepness;
        [SerializeField] private Vector3 _checkBoxHalfExtents;
        [SerializeField] private float _checkBoxUpwardOffset;
        [SerializeField] private LayerMask _checkBoxLayers;
        [SerializeField] private float _edgeIntersectionSize;
        public RequirementsTestResult result;

        [SerializeField] private GameObject[] _errorMessageGO;
        [SerializeField] private TMPro.TMP_Text[] _errorMessageText;
        [SerializeField] private string _msgTooShort;
        [SerializeField] private string _msgTooLong;
        [SerializeField] private string _msgTooSteep;
        [SerializeField] private string _msgOverlappingOthers;
        [SerializeField] private string _msgInvalidShape;
        [SerializeField] private string _msgNotEnoughResources;
        [SerializeField] private string _msgDirectionNotMatch;
        #endregion

        #region Functions
        bool TestForAllRequirements(bool isConnectionDataNotInitialized, bool isTooShort, bool isTooLong,bool isTooSteep, bool isOverlappingOthers, bool isInvalidShape, bool directionNotMatch, bool isNotEnoughResources)
        {
            return isConnectionDataNotInitialized == false && isTooLong == false && isTooShort == false && isTooSteep == false && isOverlappingOthers == false && isInvalidShape == false && directionNotMatch == false && isNotEnoughResources == false;
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
        bool TestForSteepnes(Quaternion[] rotations,Vector3[] positions)
        {
            tooStepPoints.Clear();
            Vector3[] worldPositions = OrientedPoints.PositionsLocalToWorld(positions, _previewTransform);
            bool result = false;
            for (int i = 0; i < rotations.Length; i++)
            {
                Quaternion flatRotation = Quaternion.Euler(0, rotations[i].eulerAngles.y, rotations[i].eulerAngles.z);
                if (Quaternion.Angle(rotations[i], flatRotation) > _minMaxSteepness)
                {
                    tooStepPoints.Add(worldPositions[i]);
                    result = true;
                }
            }
            return result;
        }
        Vector3 InverseTransformPoint(Vector3 currentPoint, Quaternion currentPointRotation, Vector3 nextPoint)
        {
            Vector3 difference = nextPoint - currentPoint;
            return Quaternion.Inverse(currentPointRotation) * difference;
        }
        Vector3 GetPointForIntersectionTest(Vector3 direction, Quaternion pointRotation, Vector3 pointPosition)
        {
            Vector3 worldSpaceDirection = pointRotation * direction;
            Vector3 edgeExtent = worldSpaceDirection * _edgeIntersectionSize;
            return pointPosition + edgeExtent;
        }
        bool TestForInvalidShape(OrientedPoints orientedPoints)
        {
            bool result = false;

            segmentIntersectionCurrent.Clear();
            segmentIntersectionNextA.Clear();
            segmentIntersectionNextB.Clear();

            Vector3[] worldPositions = OrientedPoints.PositionsLocalToWorld(orientedPoints.positions, _previewTransform);
            Quaternion[] worldRotations = OrientedPoints.RotationsLocalToWorld(orientedPoints.rotations, _previewTransform);
            for (int i = 0; i < orientedPoints.positions.Length - 2; i++)
            {
                Vector3 currentPoint = worldPositions[i];
                Vector3 nextPoint = worldPositions[i+1];
                Quaternion currentRotation = worldRotations[i];
                Quaternion nextRotation = worldRotations[i +1];

                Vector3 nextPointLeft = GetPointForIntersectionTest(-Vector3.right, nextRotation, nextPoint);
                Vector3 nextPointRight = GetPointForIntersectionTest(Vector3.right, nextRotation, nextPoint);

                if (InverseTransformPoint(currentPoint, currentRotation, nextPointLeft).z < 0)
                {
                    result = true;

                    segmentIntersectionCurrent.Add(currentPoint);
                    segmentIntersectionNextA.Add(nextPointLeft);
                    segmentIntersectionNextB.Add(nextPointRight);
                }
                if (InverseTransformPoint(currentPoint, currentRotation, nextPointRight).z < 0)
                {
                    result = true;

                    segmentIntersectionCurrent.Add(currentPoint);
                    segmentIntersectionNextA.Add(nextPointLeft);
                    segmentIntersectionNextB.Add(nextPointRight);
                }
            }
            return result;
        }
        bool TestForDirectionOfAttachedConveyor(ConveyorConnectionData connectionDataStart, ConveyorConnectionData connectionDataEnd)
        {
            return connectionDataStart.conveyorSide == connectionDataEnd.conveyorSide;
        }
        bool CollidersArePillarsStack(Collider[] collidersInBox)
        {         
            for (int i = 0; i < collidersInBox.Length; i++)
            {
                if (collidersInBox[i] != null)
                {
                    if (Mathf.Approximately(collidersInBox[i].transform.position.x, _conveyorStartTransform.position.x) && Mathf.Approximately(collidersInBox[i].transform.position.z, _conveyorStartTransform.position.z))
                    {
                        return true;
                    }
                    if (Mathf.Approximately(collidersInBox[i].transform.position.x, _conveyorEndTransform.position.x) && Mathf.Approximately(collidersInBox[i].transform.position.z, _conveyorEndTransform.position.z))
                    {
                        return true;
                    }
                }
              
            }
            return false;
        }
        bool TestForOverlappingOthers(OrientedPoints orientedPoints)
        {
            bool result = false;

            overlappingCenter.Clear();
            overlappingExtents.Clear();
            overlappingOrientation.Clear();

            Vector3[] worldPositions = OrientedPoints.PositionsLocalToWorld(orientedPoints.positions, _previewTransform);

            for (int i = 1; i < orientedPoints.positions.Length; i++)
            {
                Quaternion orientation =Quaternion.LookRotation(worldPositions[i] - worldPositions[i - 1]);
                Vector3 midPoint = Vector3.Lerp(worldPositions[i], worldPositions[i - 1], 0.5f);
                Vector3 upwardOffset = (orientation * Vector3.up) * _checkBoxUpwardOffset;
                Vector3 center = midPoint + upwardOffset;
                Vector3 extents = _checkBoxHalfExtents;
                float distanceBetweeenPoints = Vector3.Distance(worldPositions[i], worldPositions[i - 1]);                           
                float extentZ = distanceBetweeenPoints / 2;
                extents.z = extentZ;

                Collider[] collidersInBox = new Collider[3];
                int amountOfColliders = Physics.OverlapBoxNonAlloc(center, extents, collidersInBox, orientation);
      
                if (amountOfColliders > 0)
                {
;                    if (CollidersArePillarsStack(collidersInBox) == false)
                    {
                        result = true;
                        overlappingCenter.Add(center);
                        overlappingExtents.Add(extents);
                        overlappingOrientation.Add(orientation);
                    }
                }
            } 
            return result;
        }
        bool TestForNotEnoughResources()
        {
            return false;
        }
        #endregion



        #region Methods 
        public void Reset()
        {
            result = new RequirementsTestResult();
        }
        public void Test(OrientedPoints orientedPoints, ConveyorConnectionData connectionDataStart, ConveyorConnectionData connectionDataEnd)
        {
            result = new RequirementsTestResult
            {
                directionNotMatch = TestForDirectionOfAttachedConveyor(connectionDataStart, connectionDataEnd),
                isConnectionDataNotInitialized = TestForConnectionDataInitialization(connectionDataStart, connectionDataEnd),
                isInvalidShape = TestForInvalidShape(orientedPoints),
                isOverlappingOthers = TestForOverlappingOthers(orientedPoints),
                isNotEnoughResources = TestForNotEnoughResources(),
                isTooLong = TestForMaxLength(orientedPoints.totalDistance),
                isTooShort = TestForMinLength(orientedPoints.totalDistance),
                isTooSteep = TestForSteepnes(orientedPoints.rotations, orientedPoints.positions)
            };
            result.meetAllRequirements = TestForAllRequirements(result.isConnectionDataNotInitialized, result.isTooShort, result.isTooLong, result.isTooSteep, result.isOverlappingOthers, result.isInvalidShape, result.directionNotMatch, result.isNotEnoughResources);

            ShowResultMessage();
        }
        void ShowResultMessage()
        {
            int messageCount = -1;
            if (result.directionNotMatch)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgDirectionNotMatch;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }
            if (result.isTooLong)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgTooLong;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }
            if (result.isTooShort)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgTooShort;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }
            if (result.isTooSteep)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgTooSteep;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }
            if (result.isOverlappingOthers)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgOverlappingOthers;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }
            if (result.isInvalidShape)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgInvalidShape;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }
            if (result.isNotEnoughResources)
            {
                messageCount++;
                _errorMessageText[messageCount].text = _msgNotEnoughResources;
                if (!_errorMessageGO[messageCount].activeSelf)
                    _errorMessageGO[messageCount].SetActive(true);
            }

            HideMessages(messageCount + 1);
        }
        public void HideMessages(int startFrom = 0)
        {         
            for (int i = startFrom; i < _errorMessageGO.Length; i++)
            {
                if (_errorMessageGO[i].activeSelf)
                    _errorMessageGO[i].SetActive(false);
            }
        }
        #endregion

    }
    [System.Serializable]
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
    }
}
