using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public VertexData vertexDataRight;
    public VertexData vertexDataLeft;

    public Edge()
    {
    }

    public Edge(VertexData vertexDataRight, VertexData vertexDataLeft)
    {
        this.vertexDataRight = vertexDataRight;
        this.vertexDataLeft = vertexDataLeft;
    }
}
