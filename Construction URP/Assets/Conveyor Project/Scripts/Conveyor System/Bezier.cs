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

    [Header("Fields", order = 1)]
    public int pointCount = 8;
    [SerializeField] private Transform _previewTransform; 
    public Transform[] controlPoints;
    [HideInInspector] public OrientedPoint orientedPoints;

    public Vector3 GetPoint(float t, Transform[] controlPoint)
    {
        t = Mathf.Clamp01(t);
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return _previewTransform.InverseTransformPoint(controlPoint[0].position) * (omt2 * omt) +
               _previewTransform.InverseTransformPoint(controlPoint[1].position) * (3f * omt2 * t) +
               _previewTransform.InverseTransformPoint(controlPoint[2].position) * (3f * omt * t2) +
               _previewTransform.InverseTransformPoint(controlPoint[3].position) * (t2 * t);
    } 
    public Vector3 GetTangent(float t, Transform[] controlPoint)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 tangent =
            _previewTransform.InverseTransformPoint(controlPoint[0].position) * (-omt2) +
            _previewTransform.InverseTransformPoint(controlPoint[1].position) * (3 * omt2 - 2 * omt) +
            _previewTransform.InverseTransformPoint(controlPoint[2].position) * (-3 * t2 + 2 * t) +
            _previewTransform.InverseTransformPoint(controlPoint[3].position) * (t2);
        return tangent.normalized;
    }
    public Vector3 GetNormal3D(float t, Vector3 up, Transform[] controlPoints)
    {
        Vector3 tng = GetTangent(t,controlPoints);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    } 
    public Quaternion GetOrientation3D(float t, Vector3 up, Transform[] controlPoints)
    {
        Vector3 tng = GetTangent(t,controlPoints);
        Vector3 nrm = GetNormal3D(t, up,controlPoints);
        return Quaternion.LookRotation(tng, nrm);
    }
    public (Vector3[] position, Quaternion[] rotation) GetPositionsAndRotations(int pointCount,Transform[] controlPoints)
    {
        Vector3[] positions = new Vector3[pointCount];
        Quaternion[] rotations = new Quaternion[pointCount];
        float tSegment = 1 / ((float)pointCount - 1);
        float t = -tSegment;
        for (int i = 0; i < pointCount; i++)
        {
            t += tSegment;
            positions[i] = GetPoint(t,controlPoints);
            rotations[i] = GetOrientation3D(t, Vector3.up,controlPoints);           
        }
        return (positions, rotations);
    }
    public void AlignControlPoints(Transform targetStart,Transform targetEnd)
    {
        controlPoints[0].SetPositionAndRotation(targetStart.position,targetStart.rotation);
        controlPoints[3].SetPositionAndRotation(targetEnd.position, targetEnd.rotation);
    }
    public void Compute()
    {        
        var getPositionsAndRotations = GetPositionsAndRotations(pointCount,controlPoints);     
        orientedPoints.positions = getPositionsAndRotations.position;
        orientedPoints.rotations = getPositionsAndRotations.rotation;
    }
    public void DebugView()
    {
        if (orientedPoints.positions.Length == 0) return;
        if (debugControlPoints)
        {
            for (int i = 1; i < controlPoints.Length; i++)
            {
                Debug.DrawLine(controlPoints[i - 1].position, controlPoints[i].position, Color.magenta);      
            }         
        }
        if (debugOrientedPoints)
        {
            for (int i = 1; i < pointCount; i++)
            {
                Debug.DrawLine(orientedPoints.LocalToWorld(_previewTransform,Vector3.zero,i - 1), orientedPoints.LocalToWorld(_previewTransform, Vector3.zero, i), Color.white);    
            }          
        }
        if (debugOrientation)
        {
            for (int i = 0; i < pointCount; i++)
            {
                Debug.DrawLine(orientedPoints.LocalToWorld(_previewTransform, Vector3.zero, i), orientedPoints.LocalToWorld(_previewTransform, Vector3.up,i), Color.green);
                Debug.DrawLine(orientedPoints.LocalToWorld(_previewTransform, Vector3.zero, i), orientedPoints.LocalToWorld(_previewTransform, Vector3.right,i), Color.red);
                Debug.DrawLine(orientedPoints.LocalToWorld(_previewTransform, Vector3.zero, i), orientedPoints.LocalToWorld(_previewTransform, Vector3.forward,i), Color.blue);
            }
        }
    }
   

}
