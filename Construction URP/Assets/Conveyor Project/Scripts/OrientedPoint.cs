using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrientedPoint
{
    public Vector3[] positions;
    public Quaternion[] rotations;

    public float totalDistance;
    public float[] segmentDistanceForward;
    public float[] segmentDistanceBackward;
    public  Vector3 GetPointInLocalSpace(Vector3 vertexPosition, int index)
    {
        vertexPosition.z = 0;
        return positions[index] + rotations[index] * vertexPosition;
    }
    /*
    public  Vector3 WorldToLocal(Vector3 vertexPosition, int index)
    {
        vertexPosition.z = 0;
        return Quaternion.Inverse(rotation[index]) * (vertexPosition - position[index]);
    }
    */
    public Vector3 LocalToWorld(Transform fromOrientation,Vector3 vertexPosition, int index)
    {
        vertexPosition.z = 0;
        return fromOrientation.TransformPoint(GetPointInLocalSpace(vertexPosition, index));
    }
    public  Vector3 LocalToWorldDirection(Vector3 dir, int index)
    {
        return rotations[index] * dir;
    }
}
