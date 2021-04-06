using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTransmission
{
    [SerializeField] private bool _drawDebug;
    public bool reversedTransmission;
    public float speed;

    public Vector3[] positions;
    public AnimationCurve componentX;
    public AnimationCurve componentY;
    public AnimationCurve componentZ;

    public List<float> itemsProgress = new List<float>();
    public List<Transform> itemsTransforms = new List<Transform>();

    public float totalDistance;
    (AnimationCurve componentX, AnimationCurve componentY, AnimationCurve componentZ) GetCurvePathComponents(float[] segmentDistanceForward, Vector3[] positions)
    {
        Keyframe[] x = new Keyframe[positions.Length];
        Keyframe[] y = new Keyframe[positions.Length];
        Keyframe[] z = new Keyframe[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            x[i] = new Keyframe(segmentDistanceForward[i], positions[i].x, 0, 0, 0, 0);
            y[i] = new Keyframe(segmentDistanceForward[i], positions[i].y, 0, 0, 0, 0);
            z[i] = new Keyframe(segmentDistanceForward[i], positions[i].z, 0, 0, 0, 0);
        }

        AnimationCurve componentX = new AnimationCurve(x);
        AnimationCurve componentY = new AnimationCurve(y);
        AnimationCurve componentZ = new AnimationCurve(z);
        
        return (componentX, componentY, componentZ);
    }
    float GetStep()
    {
        return Time.deltaTime * (reversedTransmission? -speed:speed);
    }
    Vector3 GetPositionOnPath(float progress)
    {
        return new Vector3(componentX.Evaluate(progress), componentY.Evaluate(progress), componentZ.Evaluate(progress));
    }
    Vector3 GetTransformDirection(Vector3 positionOnPath, Vector3 lookAt)
    {
        return (reversedTransmission ? lookAt - positionOnPath : positionOnPath - lookAt).normalized;
    }
    Vector3 GetLookAtPosition(float progress)
    {
        return GetPositionOnPath(progress + (reversedTransmission? -0.5f:0.5f));
    }

    public void CreatePath(bool reversedTransmission, float speed,Vector3[] positions, float[] segmentDistanceForward,float totalDistance)
    {       
        this.reversedTransmission = reversedTransmission;
        this.speed = speed;
        this.positions = positions;
        this.totalDistance = totalDistance;

        var curvePathComponents = GetCurvePathComponents(segmentDistanceForward,positions);
        componentX = curvePathComponents.componentX;
        componentY = curvePathComponents.componentY;
        componentZ = curvePathComponents.componentZ;
    }
    public void SetItemProgress(int index, float progress)
    {
        itemsProgress[index] = progress;
    }
    public void AddItem(Transform itemTransform)
    {
        itemsTransforms.Add(itemTransform);
        itemsProgress.Add(new float());
    }
    public void RemoveItem(int index = 0)
    {
        itemsTransforms.RemoveAt(index);
        itemsProgress.RemoveAt(index);
    }

    public void Update()
    {
        for (int i = 0; i < itemsProgress.Count; i++)
        {
            itemsProgress[i] += GetStep();
            itemsTransforms[i].position = GetPositionOnPath(itemsProgress[i]);
            Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[i]);
            itemsTransforms[i].forward = GetTransformDirection(itemsTransforms[i].position, lookAtPosition);
        }

        if (_drawDebug)
        {
            DebugTransmission();
        }
    }
    void DebugTransmission()
    {
        for (int i = 1; i < positions.Length; i++)
        {
            Debug.DrawLine(positions[i - 1], positions[i], Color.red);
        }
    }

}
