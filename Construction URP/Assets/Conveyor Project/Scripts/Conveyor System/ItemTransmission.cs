using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTransmission
{
    public bool consecutiveExist;
    public bool canMoveToEnd;
    public bool enoughSpace;
    public bool canPass;

    [SerializeField] private bool _drawDebug;
    public bool reversedTransmission;
    public float speed;

    public Vector3[] positions;
    public AnimationCurve componentX;
    public AnimationCurve componentY;
    public AnimationCurve componentZ;

    public List<float> itemsProgress = new List<float>();
    public List<Transform> itemsTransforms = new List<Transform>();
    public List<ItemAsset> itemAssets = new List<ItemAsset>();


    public float totalDistance;
   

    private IConveyorItemGate _consecutiveFactoryOrConveyor;

    public float itemHalfwayLength;

    public ItemTransmission()
    {
    }

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
        return Time.deltaTime * (reversedTransmission? -speed:speed)/10;
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
        return GetPositionOnPath(progress + (reversedTransmission ? -0.5f : 0.5f));
    }

    float GetProgressWithMargin(int index)
    {
        float margin = 0;
        if (reversedTransmission)
        {
            margin = itemsProgress[index] - itemHalfwayLength;
        }
        else
        {
            margin = itemsProgress[index] + totalDistance - itemHalfwayLength;
        }    
        return margin;
    }
    bool IsEnaughSpaceToMove(int index)
    {
        if(itemsProgress.Count <= 1)
        {
            return true;
        }
        return itemsProgress[index + 1] - itemsProgress[index] >= itemHalfwayLength;
    }

    bool CanMoveToEnd(int index)
    {
        if (_consecutiveFactoryOrConveyor == null)
        {
            return false;
        }
        else
        {
            return _consecutiveFactoryOrConveyor.CanReceiveItem(itemAssets[index]);
        }
    }
    bool CanMoveItem(int index)
    {              
        return IsEnaughSpaceToMove(index) && CanMoveToEnd(index);
    }
    bool CanPassItem(int index)
    {
        if (reversedTransmission)
        {
            return itemsProgress[index] <= 0;
        }
        else
        {
            return itemsProgress[index] >= totalDistance;
        }
    }
    public void CreatePath(bool reversedTransmission, float speed,Vector3[] positions, float[] segmentDistanceForward,float totalDistance, float itemHalfwayLength = 0.5f)
    {       
        this.reversedTransmission = reversedTransmission;
        this.speed = speed;
        this.positions = positions;
        this.totalDistance = totalDistance;
        this.itemHalfwayLength = itemHalfwayLength;

        var curvePathComponents = GetCurvePathComponents(segmentDistanceForward,positions);
        componentX = curvePathComponents.componentX;
        componentY = curvePathComponents.componentY;
        componentZ = curvePathComponents.componentZ;

    }
    public void AssignConsecutiveItemGate(IConveyorItemGate conveyorItemGate)
    {
        _consecutiveFactoryOrConveyor = conveyorItemGate;
        consecutiveExist = true;
    }
    public void SetItemProgress(int index, float progress)
    {
        itemsProgress[index] = progress;
    }
    public void AddItem(Transform itemTransform,ItemAsset itemAsset = null)
    {
        itemsTransforms.Add(itemTransform);
        itemsProgress.Add(reversedTransmission ? totalDistance : 0f);
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
            canMoveToEnd = CanMoveToEnd(i);
            canPass = CanPassItem(i);
            enoughSpace = IsEnaughSpaceToMove(i);
            if (CanMoveItem(i))
            {
                itemsProgress[i] += GetStep();
                itemsTransforms[i].position = GetPositionOnPath(itemsProgress[i]);
                Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[i]);
                itemsTransforms[i].forward = GetTransformDirection(itemsTransforms[i].position, lookAtPosition);


                if (CanPassItem(i))
                {
                    _consecutiveFactoryOrConveyor.ReceiveItem(itemAssets[i], itemsTransforms[i]);
                    RemoveItem(i);
                }
            }
        }      
    }
    public void UpdateForRevealEffect()
    {
        for (int i = 0; i < itemsProgress.Count; i++)
        {
            itemsProgress[i] += GetStep();
            itemsTransforms[i].position = GetPositionOnPath(itemsProgress[i]);
            Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[i]);
            itemsTransforms[i].forward = GetTransformDirection(itemsTransforms[i].position, lookAtPosition);
        }
    }
    public void DebugTransmission()
    {
        for (int i = 1; i < positions.Length; i++)
        {
            Debug.DrawLine(positions[i - 1], positions[i], Color.red);
        }
        Vector3 firstItemPos = GetPositionOnPath(itemsProgress[0]);
        Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[0]);
        Debug.DrawLine(firstItemPos, lookAtPosition, Color.green);
    }

}
