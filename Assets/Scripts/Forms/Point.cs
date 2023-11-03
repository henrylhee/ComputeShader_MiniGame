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
    public int faction = new int();
    public int index = new int();
    public Point[] neighbours;
    public int invalidIndex = new int();


    public int successfulNeighbourDir = new int();

    public float4 impactValues = new float4();
    public int[] impactFactions = new int[4];

    public Point(int index, int faction, int invalidIndex)
    {
        this.index = index;
        this.faction = faction;
        this.invalidIndex = invalidIndex;
        neighbours = new Point[4];
    }

    //private IEnumerable<Vector2Int> Neighbours()
    //{
    //    yield return new Vector2Int(position.x, position.y+1);
    //    yield return new Vector2Int(position.x, position.y-1);
    //    yield return new Vector2Int(position.x+1, position.y);
    //    yield return new Vector2Int(position.x-1, position.y);
    //}

    public void GetNeighbours(Point[] neighbours)
    {
        for(int i = 0; i < 4;i++)
        {
            this.neighbours[i] = neighbours[i];
        }
    }

    public void Interact()
    {
        if (faction == 0) return;
        Debug.Log("########### Faction #######"+faction);
        for(int direction = 0; direction<4;direction++) 
        {
            Debug.Log(direction + "---"+neighbours[direction]);
            if (neighbours[direction] != null)
            {
                if (neighbours[direction].faction == 0)
                {
                    //interference[-->(direction+2)%4]!!!!!!!!!!!!!!!!!!!!!!!!!!!calculation 1 time instead of 4!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    neighbours[direction].impactValues[direction] = Random.Range(0f, GlobalSettings.Instance.factionSettings[faction-1].ExpansionRate);
                    Debug.Log(direction +" -> Set impactvalue for EMPTY field: " + neighbours[direction].impactValues[direction]);
                }
                else
                {
                    neighbours[direction].impactValues[direction] = Random.Range(0f, GlobalSettings.Instance.factionSettings[faction-1].ConquerRate);
                    Debug.Log(direction + " -> Set impactvalue for ACTIVE field: " + neighbours[direction].impactValues[direction]);
                }
                Debug.Log("impactfaction before setting: "+neighbours[direction].impactFactions[direction]);
                neighbours[direction].impactFactions[direction] = faction;
                Debug.Log("impactfaction after setting: " + neighbours[direction].impactFactions[direction]);
            }
        }
    }

    public void Evaluate(ref Color[] colorMap)
    {
        float maxImpact = 0;
        //Debug.Log("-------------------------->Evaluate: ");
        for (int i = 0; i < 4; i++)
        {
            maxImpact = impactValues[i];
            if (maxImpact < impactValues[i])
            {
                //Debug.Log("Impact value: " + impactValues[i]);
                successfulNeighbourDir = i;
            }
        }
        if (maxImpact == 0) { return; }

        faction = impactFactions[((successfulNeighbourDir+2)%4)];
        Debug.Log("index: " + index + "faction: " + (faction-1) + "--->"+ impactFactions[((successfulNeighbourDir + 2) % 4)]);
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!faction - 1
        colorMap[index] = GlobalSettings.Instance.factionSettings[faction].Color;
        //Debug.Log("Point activated! Index: " + faction + ". Color value: " + colorMap[index]);
    }
}

public class ColorMapHolder
{
    public Color[] colorMap;
    public ColorMapHolder(int size)
    {
        colorMap = new Color[size];
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
