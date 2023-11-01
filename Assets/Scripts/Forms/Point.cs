using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Point
{
    public int fraction;
    public Vector2Int position { get; private set; }

    public Point[] neighbours;
    public bool isEmpty;

    private float expansionRate;
    private float conquerRate;
    private float defenseValue;

    public float[] interference;
    public int[] interferenceFraction;

    public Point(Vector2Int position, int fraction)
    {
        this.position = position;
        this.fraction = fraction;
        isEmpty = true;
    }

    public void Interact()
    {
        for (int i = 0; i < neighbours.Length; i++)
        {
            int direction = (i + 2) % 4;
            if (neighbours[i].isEmpty)
            {
                interference[direction] = expansionRate;
            }
            else
            {
                interference[direction] = conquerRate;
            }
            interferenceFraction[direction] = fraction;
        }
    }

    public void Evaluate()
    {
        float attackValue;

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
