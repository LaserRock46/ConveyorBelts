using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTransmission
{
    [SerializeField] private bool _drawDebug;

    public float speed;
    public bool reversedTransmission;
    public Vector3[] positions;
    public AnimationCurve pathComponentX;
    public AnimationCurve pathComponentY;
    public AnimationCurve pathComponentZ;

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

    (Keyframe[] componentX, Keyframe[] componentY, Keyframe[] componentZ) GetCurvePathComponents(float[] segmentDistance, Vector3[] positions)
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

        return (x, y, z);
    }
    float GetStep()
    {
        return Time.deltaTime * speed;
    }
    Vector3 GetPositionOnPath(float progress)
    {
        return new Vector3(pathComponentX.Evaluate(progress), pathComponentY.Evaluate(progress), pathComponentZ.Evaluate(progress));
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
    float[] GetSegmentDistanceForExtent(bool isThisReversed, bool isConsecutiveReversed, Vector3[] positions, float totalDistance)
    {
        List<float> distances = new List<float>();
        distances.Add(totalDistance);

        float segmentDistance = 0;
       
        switch (isThisReversed, isConsecutiveReversed)
        {
            case (false, false):
                for (int i = 1; i < positions.Length; i++)
                {
                    segmentDistance = Vector3.Distance(positions[i], positions[i - 1]) + distances[distances.Count - 1];
                    distances.Add(segmentDistance);           
                }
                break;
            case (true, true):
                for (int i = 1; i < positions.Length; i++)
                {
                    segmentDistance = Vector3.Distance(positions[i], positions[i - 1]) + distances[distances.Count - 1];
                    distances.Add(segmentDistance);
                }

                break;
            case (false, true):
                for (int i = 1; i < positions.Length; i++)
                {
                    segmentDistance = Vector3.Distance(positions[i], positions[i - 1]) + distances[distances.Count - 1];
                    distances.Add(segmentDistance);
                }
                break;
            case (true, false):
                for (int i = 1; i < positions.Length; i++)
                {
                    segmentDistance = Vector3.Distance(positions[i], positions[i - 1]) + distances[distances.Count - 1];
                    distances.Add(segmentDistance);
                }
                break;
        }

        for (int i = 0; i < positions.Length; i++)
        {          
            GameObject test = new GameObject(distances[i].ToString());
            test.transform.position = positions[i];
        }
        return distances.ToArray();
    }

    Vector3[] GetPositionsForExtent(bool isThisReversed, bool isConsecutiveReversed, Vector3[] consecutivePositions)
    {
        int extentPositionsCount = consecutivePositions.Length <= 5 ? consecutivePositions.Length : 5;
        Vector3[] positions = new Vector3[extentPositionsCount];
     
        switch (isThisReversed, isConsecutiveReversed)
        {
            case (false, false): //ok
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = consecutivePositions[i];
                    GameObject test = new GameObject(i.ToString());
                    test.transform.position = positions[i];
                }
                break;
            case (true, true): //ok
                System.Array.Reverse(consecutivePositions);
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = consecutivePositions[i];
                    GameObject test = new GameObject(i.ToString());
                    test.transform.position = positions[i];
                }
                break;
            case (false, true):
                System.Array.Reverse(consecutivePositions);
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = consecutivePositions[i];
                    GameObject test = new GameObject(i.ToString());
                    test.transform.position = positions[i];
                }
                break;
            case (true, false)://ok
                for (int i = positions.Length - 1; i >= 0; i--)
                {
                    positions[i] = consecutivePositions[i];
                    GameObject test = new GameObject(i.ToString());
                    test.transform.position = positions[i];
                }
                break;
        }

        return positions;
    }
    public void CreatePath(bool reversedTransmission, float speed,Vector3[] positions,  OrientedPoint orientedPoints,float totalDistance, float itemHalfwayLength = 0.5f, bool clearPath = false)
    {
        if (clearPath)
        {
            pathComponentX = new AnimationCurve();
            pathComponentY = new AnimationCurve();
            pathComponentZ = new AnimationCurve();
        }

        this.reversedTransmission = reversedTransmission;
        this.speed = speed;
        this.positions = positions;
        this.totalDistance = totalDistance;
        this._itemHalfwayLength = itemHalfwayLength;

        float[] segmentDistance = reversedTransmission ? orientedPoints.segmentDistanceBackward : orientedPoints.segmentDistanceForward;
        var curvePathComponents = GetCurvePathComponents(segmentDistance,positions);
        for (int i = 0; i < curvePathComponents.componentX.Length; i++)
        {
            pathComponentX.AddKey(curvePathComponents.componentX[i]);
            pathComponentY.AddKey(curvePathComponents.componentY[i]);
            pathComponentZ.AddKey(curvePathComponents.componentZ[i]);
        }
    }
    public void ExtentPathForConsecutive(ConveyorController consecutiveConveyor)
    {
        if (consecutiveConveyor == null) return;

        bool isThisReversed = reversedTransmission;
        bool isConsecutiveReversed = consecutiveConveyor.itemTransmission.reversedTransmission;

        Vector3[] positionsForExtent = GetPositionsForExtent(isThisReversed, isConsecutiveReversed, consecutiveConveyor.itemTransmission.positions);
        float[] segmentDistanceForExtent = GetSegmentDistanceForExtent(isThisReversed, isConsecutiveReversed, positionsForExtent,totalDistance);
      
        var curveExtentPathComponents = GetCurvePathComponents(segmentDistanceForExtent, positionsForExtent);

        for (int i = 0; i < positionsForExtent.Length; i++)
        {
            pathComponentX.AddKey(curveExtentPathComponents.componentX[i]);
            pathComponentY.AddKey(curveExtentPathComponents.componentY[i]);
            pathComponentZ.AddKey(curveExtentPathComponents.componentZ[i]);
        }
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
        for (int i = 1; i < pathComponentX.length; i++)
        {
            Vector3 posStart = new Vector3();
            posStart.x = pathComponentX.keys[i - 1].value;
            posStart.y = pathComponentY.keys[i - 1].value;
            posStart.z = pathComponentZ.keys[i - 1].value;
            Vector3 posEnd = new Vector3();
            posEnd.x = pathComponentX.keys[i].value;
            posEnd.y = pathComponentY.keys[i].value;
            posEnd.z = pathComponentZ.keys[i].value;
            Debug.DrawLine(posStart, posEnd, Color.green);
        }
        Vector3 firstItemPos = GetPositionOnPath(itemsProgress[0]);
        Vector3 lookAtPosition = GetLookAtPosition(itemsProgress[0]);
        Debug.DrawLine(firstItemPos, lookAtPosition, Color.red);
    }

}