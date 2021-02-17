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
    public Vector2Int[] testPointsIndex;
    private Transform[] _thisPoints;
    private Transform[] _oppositePoints;
    private int _indexThisPoint = 0;
    private int _indexOppositePoint = 0;
    private bool _found;
    public int minBackwardPoint = 15;

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
    [SerializeField] private float _angleTolerance = 5;
    [SerializeField] private float _distanceTolerance = 0.1f;


    #endregion

    #region Functions
    bool IsOppositeAnchorOnFront(Transform thisAnchor,Transform oppositeAnchor)
    {
        return thisAnchor.InverseTransformPoint(oppositeAnchor.position).z > 0;
    }
    (int indexThisPoint,int indexOppositePoint, bool found)GetArcEndPointsIndex(Transform[] points,Transform[] oppositePoints,Transform thisAnchor, Transform oppositeAnchor)
    {

        for (int thisPoint = 0; thisPoint < points.Length; thisPoint++)
        {
            for (int oppositePoint = 0; oppositePoint < oppositePoints.Length; oppositePoint++)
            {
                if (points[thisPoint].InverseTransformPoint(oppositePoints[oppositePoint].position).x < _distanceTolerance)
                {
                    if (Quaternion.Angle(points[thisPoint].rotation, oppositePoints[oppositePoint].rotation) < _angleTolerance)
                    {
                        if (IsOppositeAnchorOnFront(thisAnchor,oppositeAnchor))
                        {
                            return (thisPoint, oppositePoint, true);
                        }
                        else if(thisPoint >= minBackwardPoint )
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
    #endregion



    #region Methods
    public void GetCircularArc(CircularArc oppositeCircularArc)
    {
        var getNearestPointsAnchors = GetNearestPoints(this, oppositeCircularArc);
        var getArcEndPointIndex = GetArcEndPointsIndex(getNearestPointsAnchors.thisAnchorPoints, getNearestPointsAnchors.oppositeAnchorPoints,selfAnchor, oppositeCircularArc.selfAnchor);
        if (getArcEndPointIndex.found)
        {
            Debug.DrawLine(getNearestPointsAnchors.thisAnchorPoints[getArcEndPointIndex.indexThisPoint].position, getNearestPointsAnchors.oppositeAnchorPoints[getArcEndPointIndex.indexOppositePoint].position, Color.red);
        }

        _thisPoints = getNearestPointsAnchors.thisAnchorPoints;
        _oppositePoints = getNearestPointsAnchors.oppositeAnchorPoints;
        _indexThisPoint = getArcEndPointIndex.indexThisPoint;
        _indexOppositePoint = getArcEndPointIndex.indexOppositePoint;
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
            Debug.DrawLine(_thisPoints[_indexThisPoint].position, _oppositePoints[_indexOppositePoint].position,Color.green);
        }
    }
    #endregion
}
