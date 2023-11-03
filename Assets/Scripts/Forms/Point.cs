using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Net;

public struct Byte4
{
    public byte x;
    public byte y;
    public byte z;
    public byte w;

    public Byte4(byte x, byte y, byte z, byte w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}

public class Point
{
    public byte faction = new byte();
    public int[] neighbourIndices = new int[4];
    public int index = new int();


    public int invalidIndex = new int();
    public byte successfulNeighbourDir = new byte();

    public float4 impactValues = new float4();
    public byte[] impactFactions = new byte[4];

    public Point(int index, byte faction, int[] neighbourIndices, int invalidIndex)
    {
        this.index = index;
        this.faction = faction;
        this.neighbourIndices = neighbourIndices;
        this.invalidIndex = invalidIndex;
    }

    //private IEnumerable<Vector2Int> Neighbours()
    //{
    //    yield return new Vector2Int(position.x, position.y+1);
    //    yield return new Vector2Int(position.x, position.y-1);
    //    yield return new Vector2Int(position.x+1, position.y);
    //    yield return new Vector2Int(position.x-1, position.y);
    //}

    public void Interact(ref Point[] points)
    {
        if (faction == 0) return;
        for(int direction = 0; direction<4;direction++) 
        {
            int neighbourIndex = neighbourIndices[direction];

            if (neighbourIndex != invalidIndex)
            {
                if (points[neighbourIndex].faction == 0)
                {
                    //interference[-->(direction+2)%4]!!!!!!!!!!!!!!!!!!!!!!!!!!!calculation 1 time instead of 4!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    points[neighbourIndex].impactValues[direction] = Random.Range(0f, GlobalSettings.Instance.factionSettings[faction].ExpansionRate);
                }
                else
                {
                    points[neighbourIndex].impactValues[direction] = Random.Range(0f, GlobalSettings.Instance.factionSettings[faction].ConquerRate);
                }
                points[neighbourIndex].impactFactions[direction] = faction;
            }
        }
    }

    public void Evaluate(Color[] colorMap)
    {
        float maxImpact = 0;
        Debug.Log("-------------------------->Evaluate: ");
        for (byte i = 0; i < 4; i++)
        {
            Debug.Log("Impact value: " + impactValues[i]);
            maxImpact = impactValues[i];
            if (maxImpact < impactValues[i])
            {
                successfulNeighbourDir = i;
            }
        }
        if (maxImpact == 0) { return; }

        faction = impactFactions[(successfulNeighbourDir+2)%4];
        colorMap[index] = GlobalSettings.Instance.factionSettings[faction].Color;
        Debug.Log("Point activated! Index: " + faction);
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
