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
    public int shortestDistIndex;
    public float[] dist;
    
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
    bool IsOppositeOnLeft(Transform oppositeAnchor)
    {
        return selfAnchor.InverseTransformPoint(oppositeAnchor.position).x < 0;
    }
    bool IsOppositeOnFront(Transform oppositeAnchor)
    {
        return selfAnchor.InverseTransformPoint(oppositeAnchor.position).z > 0;
    }
    (int indexThisPoint,int indexOppositePoint, bool found)GetArcEndPointsIndex(Transform[] points,Transform[] oppositePoints)
    {
        for (int thisPoint = 0; thisPoint < points.Length; thisPoint++)
        {
            for (int oppositePoint = 0; oppositePoint < oppositePoints.Length; oppositePoint++)
            {
                if (points[thisPoint].InverseTransformPoint(oppositePoints[oppositePoint].position).x < _distanceTolerance)
                {
                    if (Quaternion.Angle(points[thisPoint].rotation, oppositePoints[oppositePoint].rotation) < _angleTolerance)
                    {
                        return (thisPoint, oppositePoint, true);
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
        dist = allDistances;
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
        shortestDistIndex = shortestDistanceIndex;
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
        var getArcEndPointIndex = GetArcEndPointsIndex(getNearestPointsAnchors.thisAnchorPoints, getNearestPointsAnchors.oppositeAnchorPoints);
        if (getArcEndPointIndex.found)
        {
            Debug.DrawLine(getNearestPointsAnchors.thisAnchorPoints[getArcEndPointIndex.indexThisPoint].position, getNearestPointsAnchors.oppositeAnchorPoints[getArcEndPointIndex.indexOppositePoint].position, Color.red);
        }
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
        if (_debugDrawTestConnection)
        {
            Debug.DrawLine(selfAnchor.position, oppositeCircularArc.selfAnchor.position,Color.green);
        }
    }
    #endregion
}
