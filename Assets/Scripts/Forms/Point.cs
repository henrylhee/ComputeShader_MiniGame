using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int fractionId { get; private set; }
    public Vector2Int position { get; private set; }


    public Point(Vector2Int position, int fractionId)
    {
        this.position = position;
        this.fractionId = fractionId;
    }
}

[System.Serializable]
public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
