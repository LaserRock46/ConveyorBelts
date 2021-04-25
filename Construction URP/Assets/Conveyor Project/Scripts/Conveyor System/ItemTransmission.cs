using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemTransmission
{  
    [SerializeField] private bool _drawDebug;
    public float dist;
    public bool move;
    public float speed;

    public Vector3[] positions;
    public AnimationCurve componentX;
    public AnimationCurve componentY;
    public AnimationCurve componentZ;

    public List<float> itemsProgress = new List<float>();
    public List<Transform> itemsTransforms = new List<Transform>();
    public List<ItemAsset> itemAssets = new List<ItemAsset>();


    public float totalDistance;
   

    public IConveyorItemGate consecutiveFactoryOrConveyor;

    [SerializeField] private float _itemHalfwayLength;
    [SerializeField] private float _lookAtDistance = 0.2f;

    public ItemTransmission()
    {
    }

    (AnimationCurve componentX, AnimationCurve componentY, AnimationCurve componentZ) GetCurvePathComponents(float[] segmentDistance, Vector3[] positions)
    {
        Keyframe[] x = new Keyframe[positions.Length];
        Keyframe[] y = new Keyframe[positions.Length];
        Keyframe[] z = new Keyframe[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            x[i] = new Keyframe(segmentDistance[i], positions[i].x, 0, 0, 0, 0);
            y[i] = new Keyframe(segmentDistance[i], positions[i].y, 0, 0, 0, 0);
            z[i] = new Keyframe(segmentDistance[i], positions[i].z, 0, 0, 0, 0);
        }

        AnimationCurve componentX = new AnimationCurve(x);
        AnimationCurve componentY = new AnimationCurve(y);
        AnimationCurve componentZ = new AnimationCurve(z);
        
        return (componentX, componentY, componentZ);
    }
    float GetStep()
    {
        return Time.deltaTime * speed;
    }
    Vector3 GetPositionOnPath(float progress)
    {
        return new Vector3(componentX.Evaluate(progress), componentY.Evaluate(progress), componentZ.Evaluate(progress));
    }
    Vector3 GetTransformDirection(Vector3 positionOnPath, Vector3 lookAt)
    {
        return (positionOnPath - lookAt).normalized;
    }
    Vector3 GetLookAtPosition(float progress)
    {
        return GetPositionOnPath(progress + _lookAtDistance);
    }
    bool IsEnaughSpaceToMove(int index)
    {
        bool move = false;
        if (index == 0)
        {
            move = true;
        }
        else
        {
            move = itemsProgress[index - 1] - itemsProgress[index] >= _itemHalfwayLength;
        }
        itemsTransforms[index].name = move.ToString();
        return move;
    }

    bool CanMoveToEnd(int index)
    {
        bool isProgressBeforeTheEnd = totalDistance - itemsProgress[index] > _itemHalfwayLength;
        dist = totalDistance - itemsProgress[index];
        this.move = isProgressBeforeTheEnd;
        bool move = false;
        if (isProgressBeforeTheEnd)
        {      
            move = true;
        }
        else if (consecutiveFactoryOrConveyor != null)
        {
            float distanceToEnd = totalDistance - itemsProgress[index];
            move = consecutiveFactoryOrConveyor.CanReceiveItem(itemAssets[index], distanceToEnd);

        }
        return move;
    }
    bool CanMoveItem(int index)
    {              
        return IsEnaughSpaceToMove(index) && CanMoveToEnd(index);
    }
    bool CanPassItem(int index)
    {
        return itemsProgress[index] >= totalDistance;
    }
    public void CreatePath(bool reversedTransmission, float speed,Vector3[] positions,  OrientedPoint orientedPoints,float totalDistance, float itemHalfwayLength = 0.5f)
    {             
        this.speed = speed;
        this.positions = positions;          
        this.totalDistance = totalDistance;
        this._itemHalfwayLength = itemHalfwayLength;


        float[] segmentDistance = reversedTransmission ? orientedPoints.segmentDistanceBackward : orientedPoints.segmentDistanceForward;
        var curvePathComponents = GetCurvePathComponents(segmentDistance,positions);
        componentX = curvePathComponents.componentX;
        componentY = curvePathComponents.componentY;
        componentZ = curvePathComponents.componentZ;

    }
    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
    {
        consecutiveFactoryOrConveyor = conveyorItemGate;    
    }
    public void SetItemProgress(int index, float progress)
    {
        itemsProgress[index] = progress;
    }
    public void AddItem(Transform itemTransform,ItemAsset itemAsset = null)
    {
        itemsTransforms.Add(itemTransform);
        itemsProgress.Add(0f);
        itemAssets.Add(itemAsset);
    }
    public void RemoveItem(int index = 0)
    {
        itemsTransforms.RemoveAt(index);
        itemsProgress.RemoveAt(index);
        itemAssets.RemoveAt(index);
    }
    public void Update()
    {      
        for (int i = 0; i < itemsProgress.Count; i++)
        {
            if (CanMoveItem(i))
            {
                itemsProgress[i] += GetStep();
                itemsTransforms[i].position = GetPositionOnPath(itemsProgress[i]);
                Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[i]);
                Vector3 direction = GetTransformDirection(itemsTransforms[i].position, lookAtPosition);
                itemsTransforms[i].forward = direction == Vector3.zero ? itemsTransforms[i].forward : direction;


                if (CanPassItem(i))
                {
                    consecutiveFactoryOrConveyor.ReceiveItem(itemAssets[i], itemsTransforms[i]);
                    RemoveItem(i);
                }
            }
        }
        if (_drawDebug)
        {
            DebugTransmission();
        }
    }
    public void UpdateForRevealEffect()
    {
        for (int i = 0; i < itemsProgress.Count; i++)
        {
            itemsProgress[i] += GetStep();
            itemsTransforms[i].position = GetPositionOnPath(itemsProgress[i]);
            Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[i]);
            Vector3 direction = GetTransformDirection(itemsTransforms[i].position, lookAtPosition);
            itemsTransforms[i].forward = direction == Vector3.zero ? itemsTransforms[i].forward : direction;
        }
    }
    public void DebugTransmission()
    {
        if (itemsTransforms.Count == 0) return;
        for (int i = 1; i < positions.Length; i++)
        {
            Debug.DrawLine(positions[i - 1], positions[i], Color.red);
        }
        Vector3 firstItemPos = GetPositionOnPath(itemsProgress[0]);
        Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[0]);
        Debug.DrawLine(firstItemPos, lookAtPosition, Color.green);
    }

}