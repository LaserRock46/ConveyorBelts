using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrientedPoints
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
    public Vector3 LocalToWorld(Transform fromOrientation,Vector3 vertexPosition, int index)
    {
        vertexPosition.z = 0;
        return fromOrientation.TransformPoint(GetPointInLocalSpace(vertexPosition, index));
    }
    public static Vector3[] PositionsLocalToWorld(Vector3[] local, Transform self)
    {
        Vector3[] world = new Vector3[local.Length];
        for (int i = 0; i < world.Length; i++)
        {
            world[i] = self.TransformPoint(local[i]);
        }
        return world;
    }
    public static Quaternion[] RotationsLocalToWorld(Quaternion[] local, Transform self)
    {
        Quaternion[] world = new Quaternion[local.Length];
        for (int i = 0; i < world.Length; i++)
        {
            world[i] = self.rotation * local[i];
        }
        return world;
    }
}
