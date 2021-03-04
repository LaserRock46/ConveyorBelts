using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
  
    public enum IndiceLoopOrder { Front,Back}
    public IndiceLoopOrder[] triangleFromBackLoopOrder = new IndiceLoopOrder[3];
    public IndiceLoopOrder[] triangleFromFrontLoopOrder = new IndiceLoopOrder[3];
    public VertexData[] triangleFromBackLoop = new VertexData[3];
    public VertexData[] triangleFromFrontLoop = new VertexData[3];


    public Edge()
    {
    }

    public Edge(IndiceLoopOrder[] triangleFromBackLoopOrder, IndiceLoopOrder[] triangleFromFrontLoopOrder, VertexData[] triangleFromBackLoop, VertexData[] triangleFromFrontLoop)
    {
        this.triangleFromBackLoopOrder = triangleFromBackLoopOrder;
        this.triangleFromFrontLoopOrder = triangleFromFrontLoopOrder;
        this.triangleFromBackLoop = triangleFromBackLoop;
        this.triangleFromFrontLoop = triangleFromFrontLoop;
    }
}
