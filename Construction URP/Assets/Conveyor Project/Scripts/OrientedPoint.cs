using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrientedPoint
{
    public Vector3[] position;
    public Quaternion[] rotation;


    public static Vector3 LocalToWorld(Vector3 vertexPosition, Vector3 position, Quaternion rotation)
    {
        vertexPosition.z = 0;
        return position + rotation * vertexPosition;
    }
    public static Vector3 WorldToLocal(Vector3 vertexPosition, Vector3 position, Quaternion rotation)
    {
        return Quaternion.Inverse(rotation) * (vertexPosition - position);
    }
    public static Vector3 LocalToWorldDirection(Vector3 dir, Quaternion rotation )
    {
        return rotation * dir;
    }

    public  Vector3 LocalToWorld(Vector3 vertexPosition, int index)
    {
        vertexPosition.z = 0;
        return position[index] + rotation[index] * vertexPosition;
    }
    public  Vector3 WorldToLocal(Vector3 vertexPosition, int index)
    {
        return Quaternion.Inverse(rotation[index]) * (vertexPosition - position[index]);
    }
    public  Vector3 LocalToWorldDirection(Vector3 dir, int index)
    {
        return rotation[index] * dir;
    }
}
