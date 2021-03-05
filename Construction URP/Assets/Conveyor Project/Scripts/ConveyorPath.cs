using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorPath
{

    #region Temp
    [Header("Temporary Things", order = 0)]
    [SerializeField] private bool _drawPathDebug;
    public Transform[] arcStart;
    public Transform[] arcEnd;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    [HideInInspector] public OrientedPoint orientedPoints;
    public Transform previewTransform;
   
    public CircularArc circularArcStart;
    public CircularArc circularArcEnd;
    public Bezier bezier;
    #endregion

    #region Functions
    (Vector3[] positions, Quaternion[] rotations) GetPositionsAndRotations(Transform orientation, Transform[] loopTransforms)
    {
        Vector3[] positions = new Vector3[loopTransforms.Length];
        Quaternion[] rotations = new Quaternion[loopTransforms.Length];

        for (int i = 0; i < loopTransforms.Length; i++)
        {
            positions[i] = orientation.InverseTransformPoint(loopTransforms[i].position);
            rotations[i] = Quaternion.Inverse(orientation.rotation) * loopTransforms[i].rotation;
        }
        return (positions, rotations);
    }
    (float totalDistance, float[] segmentDistanceForward, float[] segmentDistanceBackward) GetSegmentsLength(Vector3[] positions)
    {
        float totalDistanceForward = 0;
        float totalDistanceBackward = 0;
        float[] segmentDistanceForward = new float[positions.Length];
        float[] segmentDistanceBackward = new float[positions.Length];

        for (int i = 1; i < positions.Length; i++)
        {
            float distanceForward = Vector3.Distance(positions[i - 1], positions[i]);

            totalDistanceForward += distanceForward;
            segmentDistanceForward[i] = totalDistanceForward;
        }
        for (int i = positions.Length - 2; i >= 0; i--)
        {
            float distanceBackward = Vector3.Distance(positions[i + 1], positions[i]);
            totalDistanceBackward += distanceBackward;
            segmentDistanceBackward[i] = totalDistanceBackward;
        }
        return (totalDistanceForward, segmentDistanceForward, segmentDistanceBackward);
    }
   
    #endregion



    #region Methods
    public void ConstructPath()
    {           
        circularArcStart.GetCircularArcIndexPoints(circularArcEnd);
        bezier.AlignControlPoints(circularArcStart.GetThisArcPointEnd(),circularArcStart.GetOppositeArcPointStart());
        bezier.Compute();
        GetOrientedPoints();
        GetPathLength();
    }
    void GetOrientedPoints()
    {
        Transform[] arcStart = circularArcStart.GetArcPoints(circularArcStart.thisPoints,circularArcStart.indexThisPoint, circularArcStart.order);
        Transform[] arcEnd = circularArcStart.GetArcPoints(circularArcStart.oppositePoints, circularArcStart.indexOppositePoint, circularArcEnd.order);
        this.arcStart = arcStart;
        this.arcEnd = arcEnd;

        var getPositionsAndRotationsArcStart = GetPositionsAndRotations(previewTransform, arcStart);
        var getPositionsAndRotationsArcEnd = GetPositionsAndRotations(previewTransform, arcEnd);

        List<Vector3> allPositions = new List<Vector3>();
        List<Quaternion> allRotations = new List<Quaternion>();

        allPositions.AddRange(getPositionsAndRotationsArcStart.positions);
        allPositions.AddRange(bezier.orientedPoints.positions);
        allPositions.AddRange(getPositionsAndRotationsArcEnd.positions);

        allRotations.AddRange(getPositionsAndRotationsArcStart.rotations);
        allRotations.AddRange(bezier.orientedPoints.rotations);
        allRotations.AddRange(getPositionsAndRotationsArcEnd.rotations);

        orientedPoints.positions = allPositions.ToArray();
        orientedPoints.rotations = allRotations.ToArray();
    }
    void GetPathLength()
    {
        var getSegmentsLength = GetSegmentsLength(orientedPoints.positions);
        orientedPoints.segmentDistanceForward = getSegmentsLength.segmentDistanceForward;
        orientedPoints.segmentDistanceBackward = getSegmentsLength.segmentDistanceBackward;
        orientedPoints.totalDistance = getSegmentsLength.totalDistance;
    }
    public void DebugDraw()
    {
        if (!_drawPathDebug) return;

        circularArcStart.DebugCircles(circularArcEnd);
        circularArcEnd.DebugCircles(circularArcStart);

        for (int i = 1; i < orientedPoints.positions.Length; i++)
        {
            Debug.DrawLine(orientedPoints.LocalToWorld(previewTransform, Vector3.zero, i - 1), orientedPoints.LocalToWorld(previewTransform, Vector3.zero, i), Color.green);
        }
    }
    
    #endregion

}