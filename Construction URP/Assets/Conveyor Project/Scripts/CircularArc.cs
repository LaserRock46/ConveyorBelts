using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircularArc
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    [SerializeField] private bool _debugCircles;
    [SerializeField] private bool _debugDrawTestConnection;
    [SerializeField] private float _testAngleTolerance;
 
    private bool _found;

    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public StartingOrder order;
    public enum StartingOrder { Start,End}
    public Transform selfAnchor;
    public Transform pointsLeftAnchor;
    public Transform pointsRightAnchor;
    public Transform[] pointsLeft;
    public Transform[] pointsRight;

    public Transform[] thisPoints;
    public Transform[] oppositePoints;
    public int indexThisPoint = 0;
    public int indexOppositePoint = 0;

    [SerializeField] private float _angleTolerance = 5;
    [SerializeField] private float _distanceTolerance = 0.1f;
    [SerializeField] private int _minBackwardPoint = 9;
    [SerializeField] private int _pointsLimit = 26;
  

    #endregion

    #region Functions
    bool IsOppositeAnchorOnFront(Transform thisAnchor,Transform oppositeAnchor)
    {
        return thisAnchor.InverseTransformPoint(oppositeAnchor.position).z > 0;
    }
    (int indexThisPoint,int indexOppositePoint, bool found)GetArcEndPointsIndex(Transform[] points,Transform[] oppositePoints,Transform thisAnchor, Transform oppositeAnchor)
    {

        for (int thisPoint = 0; thisPoint < _pointsLimit; thisPoint++)
        {
            for (int oppositePoint = 0; oppositePoint < _pointsLimit; oppositePoint++)
            {
                if (points[thisPoint].InverseTransformPoint(oppositePoints[oppositePoint].position).x < _distanceTolerance)
                {
                    if (Quaternion.Angle(points[thisPoint].rotation, oppositePoints[oppositePoint].rotation) < _angleTolerance)
                    {
                        _testAngleTolerance = Quaternion.Angle(points[thisPoint].rotation, oppositePoints[oppositePoint].rotation);
                        if (IsOppositeAnchorOnFront(thisAnchor,oppositeAnchor))
                        {
                            return (thisPoint, oppositePoint, true);
                        }
                        else if(thisPoint >= _minBackwardPoint )
                        {
                            return (thisPoint, oppositePoint, true);
                        }
                    }
                }
            }
        }
        return (0,0,false);
    }

    (Transform[] thisAnchorPoints, Transform[] oppositeAnchorPoints) GetNearestPoints(CircularArc thisCircularArc,CircularArc oppositeCircularArc)
    {
        float thisLeftToOppositeLeft = Vector3.Distance(thisCircularArc.pointsLeftAnchor.position,oppositeCircularArc.pointsLeftAnchor.position);
        float thisLeftToOppositeRight = Vector3.Distance(thisCircularArc.pointsLeftAnchor.position, oppositeCircularArc.pointsRightAnchor.position);
        float thisRightToOppositeLeft = Vector3.Distance(thisCircularArc.pointsRightAnchor.position, oppositeCircularArc.pointsLeftAnchor.position);
        float thisRightToOppositeRight = Vector3.Distance(thisCircularArc.pointsRightAnchor.position, oppositeCircularArc.pointsRightAnchor.position);
        float[] allDistances = new float[] { thisLeftToOppositeLeft, thisLeftToOppositeRight, thisRightToOppositeLeft, thisRightToOppositeRight };

        float shortestDistance = Mathf.Infinity;
        int shortestDistanceIndex = 0;
        for (int i = 0; i < allDistances.Length; i++)
        {
            if(allDistances[i] <= shortestDistance)
            {
                shortestDistance = allDistances[i];
                shortestDistanceIndex = i;              
            }
        }
      
        switch (shortestDistanceIndex)
        {
            case 0:
                return (thisCircularArc.pointsLeft, oppositeCircularArc.pointsLeft);
            case 1:
                return (thisCircularArc.pointsLeft, oppositeCircularArc.pointsRight);
            case 2:
                return (thisCircularArc.pointsRight, oppositeCircularArc.pointsLeft);
            case 3:
                return (thisCircularArc.pointsRight, oppositeCircularArc.pointsRight);
        }
        return (null, null);
    }
    public Transform[] GetArcPoints(Transform[] pointsFromAnchor, int indexPoint, StartingOrder startingOrder)
    {
        List<Transform> arcPoints = new List<Transform>();
        if(startingOrder == StartingOrder.Start)
        {
            for (int i = 0; i <= indexPoint; i++)
            {
                arcPoints.Add(pointsFromAnchor[i]);
            }
        }
        else
        {
            for (int i = indexPoint; i >= 0; i--)
            {
                arcPoints.Add(pointsFromAnchor[i]);
            }
        }
        return arcPoints.ToArray();
    }
    public Transform GetThisArcPointEnd()
    {
        return thisPoints[indexThisPoint];
    }
    public Transform GetOppositeArcPointStart()
    {
        return oppositePoints[indexOppositePoint];
    }
    #endregion



    #region Methods
    public void GetCircularArcIndexPoints(CircularArc oppositeCircularArc)
    {
        var getNearestPointsAnchors = GetNearestPoints(this, oppositeCircularArc);
        var getArcEndPointIndex = GetArcEndPointsIndex(getNearestPointsAnchors.thisAnchorPoints, getNearestPointsAnchors.oppositeAnchorPoints,selfAnchor, oppositeCircularArc.selfAnchor);
        if (getArcEndPointIndex.found)
        {        
            thisPoints = getNearestPointsAnchors.thisAnchorPoints;
            oppositePoints = getNearestPointsAnchors.oppositeAnchorPoints;
            indexThisPoint = getArcEndPointIndex.indexThisPoint;
            indexOppositePoint = getArcEndPointIndex.indexOppositePoint;
        }

        _found = getArcEndPointIndex.found;
    }
    public void DebugCircles(CircularArc oppositeCircularArc)
    {
        if (_debugCircles)
        {
            for (int i = 1; i < pointsLeft.Length; i++)
            {
                Debug.DrawLine(pointsLeft[i].position, pointsLeft[i - 1].position,Color.white);
                Debug.DrawLine(pointsRight[i].position, pointsRight[i - 1].position, Color.white);
            }
        }
        if (_debugDrawTestConnection && _found)
        {
            Debug.DrawLine(thisPoints[indexThisPoint].position, oppositePoints[indexOppositePoint].position,Color.green);
        }
    }
    #endregion
}
