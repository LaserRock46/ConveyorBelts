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
    public Transform[] controlPointsB1;
    public Transform[] controlPointsB2;
    public OrientedPoint orientedPoints;

    public float totalDistance;
    public float[] segmentDistanceForward;
    public float[] segmentDistanceBackward;

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
    public (float totalDistance, float[] segmentDistanceForward, float[] segmentDistanceBackward) GetSegmentsLength()
    {
        float totalDistanceForward = 0;
        float totalDistanceBackward = 0;
        float[] segmentDistanceForward = new float[pointCount];
        float[] segmentDistanceBackward = new float[pointCount];

        for (int i = 1; i < pointCount; i++)
        {
            float distanceForward = Vector3.Distance(orientedPoints.position[i - 1], orientedPoints.position[i]);

            totalDistanceForward += distanceForward;
            segmentDistanceForward[i] = totalDistanceForward;
        }
        for (int i = pointCount - 2; i >= 0; i--)
        {
            float distanceBackward = Vector3.Distance(orientedPoints.position[i + 1], orientedPoints.position[i]);
            totalDistanceBackward += distanceBackward;
            segmentDistanceBackward[i] = totalDistanceBackward;
        }
        return (totalDistanceForward, segmentDistanceForward, segmentDistanceBackward);
    }
    public Vector3 GetBezierConnectionPosition()
    {
        return Vector3.Lerp(controlPointsB1[2].position, controlPointsB2[1].position,0.5f);
    }
    public void Compute()
    {
        Vector3 connectionPosition = GetBezierConnectionPosition();
        //controlPointsB1[3].position = connectionPosition;
        //controlPointsB2[0].position = connectionPosition;
        controlPointsB1[2].LookAt(controlPointsB2[1]);
        controlPointsB2[1].LookAt(2 * controlPointsB2[1].position - controlPointsB1[2].position);

        var getPositionsAndRotationsB1 = GetPositionsAndRotations(pointCount/2,controlPointsB1);
        var getPositionsAndRotationsB2 = GetPositionsAndRotations(pointCount/2,controlPointsB2);

        List<Vector3> allPositions = new List<Vector3>();
        List<Quaternion> allRotations = new List<Quaternion>();
        allPositions.AddRange(getPositionsAndRotationsB1.position);
        allPositions.AddRange(getPositionsAndRotationsB2.position);
        allRotations.AddRange(getPositionsAndRotationsB1.rotation);
        allRotations.AddRange(getPositionsAndRotationsB2.rotation);
        orientedPoints.position = allPositions.ToArray();
        orientedPoints.rotation = allRotations.ToArray();


        var getSegmentsLength = GetSegmentsLength();
        totalDistance = getSegmentsLength.totalDistance;
        segmentDistanceForward = getSegmentsLength.segmentDistanceForward;
        segmentDistanceBackward = getSegmentsLength.segmentDistanceBackward;

    }
  
    public void DebugView()
    {
        if (orientedPoints.position.Length == 0) return;
        if (debugControlPoints)
        {
            for (int i = 1; i < controlPointsB1.Length; i++)
            {
                Debug.DrawLine(controlPointsB1[i - 1].position, controlPointsB1[i].position, Color.magenta);
                Debug.DrawLine(controlPointsB2[i - 1].position, controlPointsB2[i].position, Color.magenta);
            }
            /*
            for (int i = 0; i < controlPointTransform.Length; i++)
            {
                controlPointTransform[i].name = controlPointTransform[i].position.y.ToString("F2");
            }
            */
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
