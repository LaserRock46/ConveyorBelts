using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EdgeLoop
{
    public VertexData[] vertexDatas;
    public Edge[] edges;

    public EdgeLoop()
    {
    }

    public EdgeLoop(VertexData[] vertexDatas, Edge[] edges)
    {
        this.vertexDatas = vertexDatas;
        this.edges = edges;
    }
}
