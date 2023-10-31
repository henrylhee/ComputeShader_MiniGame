using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int fractionId;
    public Vector2Int position { get; private set; }
    public int index { get; private set; }


    public Point(Vector2Int position, int fractionId, int index)
    {
        this.position = position;
        this.fractionId = fractionId;
        this.index = index;
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
