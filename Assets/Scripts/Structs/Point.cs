using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Net;


struct Point
{
    public int index;
    public int faction;
    public bool isActive;
    public int4 neighbourIndices;

    public Point(int index, int faction, bool isActive, int4 neighbourIndices)
    {
        this.index = index;
        this.faction = faction;
        this.isActive = isActive;
        this.neighbourIndices = neighbourIndices;
    }
};


//public class Point
//{
//    public short faction = new short();
//    public int index = new int();
//    public Point[] neighbours = new Point[4];
//    public int invalidIndex = new int();


//    public short successfulNeighbourDir = new short();

//    public float[] impactValues = new float[4] { 0, 0, 0, 0 };
//    public short[] impactFactions = new short[4] { 0,0,0,0};

//    public bool isActive = false;


//    public Point(int index, short faction, int invalidIndex)
//    {
//        this.index = index;
//        this.faction = faction;
//        this.invalidIndex = invalidIndex;
//    }

//    public void GetNeighbour(ref Point neighbour, short direction)
//    {
//        this.neighbours[direction] = neighbour;
//    }

//    public void Interact()
//    {
//        if (faction == 0) { return; }
//        if (!isActive) { return; }
//        //Debug.Log("########### Faction #######"+faction);
//        //Debug.Log("########### Index #########" + index);
//        for (int direction = 0; direction<4;direction++) 
//        {
//            if (neighbours[direction] != null)
//            {
//                if (neighbours[direction].faction == 0)
//                {
//                    //Saving the values in the opposite direction index to minimize computations!!!!!!!!!!!!!
//                    neighbours[direction].impactValues[direction] = Random.Range(0f, GlobalSettings.Instance.factionSettings[faction-1].ExpansionRate);
//                    //Debug.Log(direction +" -> Set impactvalue for EMPTY field: " + neighbours[direction].impactValues[direction]+" in direction: " + direction);
//                }
//                else
//                {
//                    neighbours[direction].impactValues[direction] = Random.Range(0f, GlobalSettings.Instance.factionSettings[faction-1].ConquerRate);
//                    //Debug.Log(direction + " -> Set impactvalue for ACTIVE field: " + neighbours[direction].impactValues[direction]);
//                }
//                //Debug.Log("impactfaction before setting: "+neighbours[direction].impactFactions[direction]);
//                neighbours[direction].impactFactions[direction] = faction;
//                neighbours[direction].isActive = true;
//                //Debug.Log("impactfaction after setting: " + neighbours[direction].impactFactions[direction]);
//            }
//        }
//    }

//    public void Evaluate(ref Color[] colorMap)
//    {
//        if(!isActive) { return; }
//        if (!IsActive()) { isActive = false; return; }

//        float maxImpact = 0;
//        for (short i = 0; i < 4; i++)
//        {
//            if (maxImpact < impactValues[i])
//            {
//                maxImpact = impactValues[i];
//                successfulNeighbourDir = i;
//            }
//        }
//        if (maxImpact == 0) { return; }

//        faction = impactFactions[successfulNeighbourDir];
//        colorMap[index] = GlobalSettings.Instance.factionSettings[faction-1].Color;
//    }

//    private bool IsActive()
//    {
//        foreach (Point neighbour in neighbours)
//        {
//            if (neighbour == null) continue;
//            if (neighbour.faction != faction)
//            {
//                return true;
//            }
//        }
//        return false;
//    }
//}


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
