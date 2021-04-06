using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationCurvePaths : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public Transform[] samplePath;
    public AnimationCurve componentX;
    public AnimationCurve componentY;
    public AnimationCurve componentZ;
    public float speed;
    public float progress;
    public float totalDistance;
    public float[] distanceThreshold;
    public float targetSpeed;

  
    public Transform target;

    public float inTangent, outTangent, inWeight, outWeight;
    #endregion

    #region Functions
    (float distance,float[] threshold) GetTotalDistance(Transform[] path)
    {
        float distance = 0;
        float[] threshold = new float[path.Length];
        for (int i = 1; i < path.Length; i++)
        {
            distance += Vector3.Distance(path[i - 1].position, path[i].position);
            threshold[i] = distance;
        }
        return (distance,threshold);
    }
    float GetTargetSpeed(float speed,float distance)
    {
        return speed / distance;
    }
   
    #endregion



    #region Methods
    void Awake()
    {
        ComputeComponents();
        
    }
   void Update()
    {
        UpdatePosition();
    }
    [ContextMenu("Recalculate")]
    void ComputeComponents()
    {
        var getTotalDistance = GetTotalDistance(samplePath);
        totalDistance = getTotalDistance.distance;
        distanceThreshold = getTotalDistance.threshold;
        Keyframe[] x = new Keyframe[samplePath.Length];
        Keyframe[] y = new Keyframe[samplePath.Length];
        Keyframe[] z = new Keyframe[samplePath.Length];
        for (int i = 0; i < samplePath.Length; i++)
        {
            x[i] = new Keyframe(distanceThreshold[i], samplePath[i].position.x, inTangent, outTangent, inWeight, outWeight);
            y[i] = new Keyframe(distanceThreshold[i], samplePath[i].position.y, inTangent, outTangent, inWeight, outWeight);
            z[i] = new Keyframe(distanceThreshold[i], samplePath[i].position.z, inTangent, outTangent, inWeight, outWeight);       
        }
        componentX.keys = x;
        componentY.keys = y;
        componentZ.keys = z;
    }
   
    void UpdatePosition()
    {
        progress += speed * Time.deltaTime;
        target.position = new Vector3(componentX.Evaluate(progress), componentY.Evaluate(progress), componentZ.Evaluate(progress));
        Vector3 lookAt = new Vector3(componentX.Evaluate(progress+0.5f), componentY.Evaluate(progress + 0.5f), componentZ.Evaluate(progress + 0.5f));
        target.LookAt(lookAt); 

    }
    #endregion

}
