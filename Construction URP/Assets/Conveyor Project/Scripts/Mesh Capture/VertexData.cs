using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VertexData
{
    public int originalVertexIndex;
    public int vertexIndex;
    public Vector3 vertexPosition;
    public Vector2 vertexUV;
    public Color32 vertexColor;

    public VertexData()
    {
    }

    public VertexData(int originalVertexIndex, int vertexIndex, Vector3 vertexPosition, Vector2 vertexUV, Color32 vertexColor)
    {
        this.originalVertexIndex = originalVertexIndex;
        this.vertexIndex = vertexIndex;
        this.vertexPosition = vertexPosition;
        this.vertexUV = vertexUV;
        this.vertexColor = vertexColor;
    }
}
