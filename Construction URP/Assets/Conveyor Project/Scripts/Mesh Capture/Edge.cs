using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public VertexData vertexDataRight;
    public VertexData vertexDataLeft;
    public enum WindingOrder { LeftBack,RigthBack,LeftFront,RightFront}
    public WindingOrder[] fromFrontLoopTriangle;
    public WindingOrder[] fromBackLoopTriangle;
    public Edge()
    {
    }

    public Edge(VertexData vertexDataRight, VertexData vertexDataLeft, WindingOrder[] fromFrontLoopTriangle, WindingOrder[] fromBackLoopTriangle)
    {
        this.vertexDataRight = vertexDataRight;
        this.vertexDataLeft = vertexDataLeft;
        this.fromFrontLoopTriangle = fromFrontLoopTriangle;
        this.fromBackLoopTriangle = fromBackLoopTriangle;
    }
}
