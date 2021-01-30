using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrientedPoint
{
    public Vector3[] position;
    public Quaternion[] rotation;


    public static Vector3 LocalToWorld(Vector3 point, Vector3 position, Quaternion rotation)
    {
        point.z = 0;
        return position + rotation * point;
    }
    public static Vector3 WorldToLocal(Vector3 point, Vector3 position, Quaternion rotation)
    {
        return Quaternion.Inverse(rotation) * (point - position);
    }
    public static Vector3 LocalToWorldDirection(Vector3 dir, Quaternion rotation )
    {
        return rotation * dir;
    }

    public  Vector3 LocalToWorld(Vector3 point, int index)
    {
        point.z = 0;
        return position[index] + rotation[index] * point;
    }
    public  Vector3 WorldToLocal(Vector3 point, int index)
    {
        return Quaternion.Inverse(rotation[index]) * (point - position[index]);
    }
    public  Vector3 LocalToWorldDirection(Vector3 dir, int index)
    {
        return rotation[index] * dir;
    }
}
