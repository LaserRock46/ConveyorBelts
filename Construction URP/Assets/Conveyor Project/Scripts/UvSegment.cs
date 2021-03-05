using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UvSegment
{
    public int start;
    public int end;
    public int pathIndex;
    public bool reverseDirection;

    public UvSegment(int start, int end, int pathIndex, bool reverseDirection)
    {
        this.start = start;
        this.end = end;
        this.pathIndex = pathIndex;
        this.reverseDirection = reverseDirection;
    }
}
