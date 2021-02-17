using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConveyorPath
{

    #region Temp
    [Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public OrientedPoint orientedPoints;
    public Transform previewTransform;
   
    public CircularArc circularArcStart;
    public CircularArc circularArcEnd;
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
    public void Compute()
    {           
        circularArcStart.GetCircularArc(circularArcEnd);
    }
    public void DebugDraw()
    {
        circularArcStart.DebugCircles(circularArcEnd);
        circularArcEnd.DebugCircles(circularArcStart);
    }
    
    #endregion

}
