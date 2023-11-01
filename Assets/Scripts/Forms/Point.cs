using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Point
{
    public int faction;
    public int[] neighbourIndices;
    public int index;


    public int invalidIndex;
    public int successfulNeighbourDir;

    private float expansionRate;
    private float conquerRate;
    private float defenseValue;

    public float[] interference;
    public int[] interferenceFaction;

    public Point(int index, int faction, int[] neighbourIndices)
    {
        this.index = index;
        this.faction = faction;
        this.neighbourIndices = neighbourIndices;
    }

    //private IEnumerable<Vector2Int> Neighbours()
    //{
    //    yield return new Vector2Int(position.x, position.y+1);
    //    yield return new Vector2Int(position.x, position.y-1);
    //    yield return new Vector2Int(position.x+1, position.y);
    //    yield return new Vector2Int(position.x-1, position.y);
    //}

    public void Interact(Point[] points)
    {
        if (faction == 0) return;
        int neighbourIndex;

        for (int direction = 0; direction < neighbourIndices.Length; direction++)
        {
            neighbourIndex = neighbourIndices[direction];
            if (neighbourIndex == invalidIndex) continue;

            if (points[neighbourIndex].faction == 0)
            {
                //interference[-->(direction+2)%4]!!!!!!!!!!!!!!!!!!!!!!!!!!!calculation 1 time instead of 4!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                points[neighbourIndex].interference[direction] = Random.Range(0f, FactionSettings[faction].expansionRate);
            }
            else
            {
                points[neighbourIndex].interference[direction] = Random.Range(0f, FactionSettings[faction].conquerRate);
            }
            points[neighbourIndex].interferenceFaction[direction] = faction;
        }
    }

    public void Evaluate()
    {
        float attackValue = 0;
        for (int i = 0; i < interference.Length; i++)
        {
            attackValue = interference[i];
            if (attackValue > interference[i])
            {
                successfulNeighbourDir = i;
            }
        }
        if (attackValue == 0) { return; }
        faction = interferenceFaction[(successfulNeighbourDir+2)%4];
        colorMap[index] = FactionSettings[faction].color;
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
