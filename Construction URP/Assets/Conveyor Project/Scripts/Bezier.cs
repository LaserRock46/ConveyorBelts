using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bezier
{
    [Header("Temporary Things", order = 0)]
    public bool debugControlPoints;
    public bool debugOrientedPoints;
    public bool debugOrientation;
    public int updateCount;
    [Header("----------------", order = 1)]
    public int pointCount = 8;
    public Transform[] controlPointTransform;    
    public OrientedPoint orientedPoints;

    public float totalDistance;
    public float[] segmentDistance;
    
    public Vector3 GetPoint(float t)
    {
        t = Mathf.Clamp01(t);
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return controlPointTransform[0].position * (omt2 * omt) +
               controlPointTransform[1].position * (3f * omt2 * t) +
               controlPointTransform[2].position * (3f * omt * t2) +
               controlPointTransform[3].position * (t2 * t);
    } 
    public Vector3 GetTangent(float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 tangent =
            controlPointTransform[0].position * (-omt2) +
            controlPointTransform[1].position * (3 * omt2 - 2 * omt) +
            controlPointTransform[2].position * (-3 * t2 + 2 * t) +
            controlPointTransform[3].position * (t2);
        return tangent.normalized;
    }
    public Vector3 GetNormal3D(float t, Vector3 up)
    {
        Vector3 tng = GetTangent(t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    } 
    public Quaternion GetOrientation3D(float t, Vector3 up)
    {
        Vector3 tng = GetTangent(t);
        Vector3 nrm = GetNormal3D(t, up);
        return Quaternion.LookRotation(tng, nrm);
    }
    public (Vector3[] position, Quaternion[] rotation) GetPositionsAndRotations()
    {
        Vector3[] positions = new Vector3[pointCount];
        Quaternion[] rotations = new Quaternion[pointCount];
        float tSegment = 1 / ((float)pointCount - 1);
        float t = -tSegment;
        for (int i = 0; i < pointCount; i++)
        {
            t += tSegment;
            positions[i] = GetPoint(t);
            rotations[i] = GetOrientation3D(t, Vector3.up);           
        }
        return (positions, rotations);
    }
    public (float totalDistance, float[] segmentDistance) GetSegmentsLength()
    {
        float totalDistance = 0;
        float[] segmentDistance = new float[pointCount];

        for (int i = 1; i < pointCount; i++)
        {
            segmentDistance[i] = Vector3.Distance(orientedPoints.position[i - 1], orientedPoints.position[i]);
            totalDistance += segmentDistance[i];
        }
        return (totalDistance, segmentDistance);
    }
    public void Compute()
    {
       
        var getPositionsAndRotations = GetPositionsAndRotations();
        orientedPoints.position = getPositionsAndRotations.position;
        orientedPoints.rotation = getPositionsAndRotations.rotation;

        var getSegmentsLength = GetSegmentsLength();
        totalDistance = getSegmentsLength.totalDistance;
        segmentDistance = getSegmentsLength.segmentDistance;

        updateCount++;
    }
  
    public void DebugView()
    {
        if (debugControlPoints)
        {
            for (int i = 1; i < controlPointTransform.Length; i++)
            {
                Debug.DrawLine(controlPointTransform[i - 1].position, controlPointTransform[i].position, Color.magenta);                
            }
            for (int i = 0; i < controlPointTransform.Length; i++)
            {
                controlPointTransform[i].name = controlPointTransform[i].position.y.ToString("F2");
            }
        }
        if (debugOrientedPoints)
        {
            for (int i = 1; i < pointCount; i++)
            {
                Debug.DrawLine(orientedPoints.position[i - 1], orientedPoints.position[i], Color.white);    
            }          
        }
        if (debugOrientation)
        {
            for (int i = 0; i < pointCount; i++)
            {
                Debug.DrawLine(orientedPoints.position[i], orientedPoints.LocalToWorld(Vector3.up,i), Color.green);
                Debug.DrawLine(orientedPoints.position[i], orientedPoints.LocalToWorld(Vector3.right,i), Color.red);
                Debug.DrawLine(orientedPoints.position[i], orientedPoints.LocalToWorld(Vector3.forward,i), Color.blue);

            }
        }
    }
   

}
