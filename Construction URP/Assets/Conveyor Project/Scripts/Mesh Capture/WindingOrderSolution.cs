using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindingOrderSolution
{  
    public Vector3 v0;
    public Vector3 v1;
    public Vector3 v2;

    public Edge.WindingOrder order0;
    public Edge.WindingOrder order1;
    public Edge.WindingOrder order2;

    public WindingOrderSolution(Vector3 v0, Vector3 v1, Vector3 v2, Edge.WindingOrder order0, Edge.WindingOrder order1, Edge.WindingOrder order2)
    {
        this.v0 = v0;
        this.v1 = v1;
        this.v2 = v2;
        this.order0 = order0;
        this.order1 = order1;
        this.order2 = order2;
    }
}
