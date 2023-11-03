
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using System;
using UnityEditor.Rendering.LookDev;
using UnityEngine.UIElements;

public class Map
{
    //problems to tackle: order of executions in pointlist???


    public Color[] colorMap;
    private Point[] points;

    public int[] ids { get; private set; }

    private int size;
    private int resoX;
    private int resoY;

    readonly int[] helperX = new int[4] {0,1,0,-1};
    readonly int[] helperY = new int[4] {1,0,-1,0};


    public void Initialize()
    {
        resoX = GameModel.Instance.resolution.width;
        resoY = GameModel.Instance.resolution.height;
        size = resoX * resoY;
        Debug.Log("width resolution: " + resoX);
        Debug.Log("height resolution: " + resoY);
        Debug.Log("array size: " + size);
        points = new Point[size];

        Color initColor = new Color(0, 0, 0, 0);
        int emptyId = 0;

        colorMap = new Color[size];
        ids = new int[size];
        colorMap = new Color[size];
        
        for (int i = 0; i < size; i++)
        {
            colorMap[i] = initColor;
            ids[i] = emptyId;
            points[i] = new Point(i, 0, size);
        }

        GenerateStartPositions();

        Point[] neighbours = new Point[4];
        int[] neighbourIndices;
        for (int i = 0; i < size; i++)
        {
            neighbourIndices = GetNeighbourIndices(i);
            for (int j = 0; j < 4; j++)
            {
                if (neighbourIndices[j] != size)
                {
                    neighbours[j] = points[neighbourIndices[j]];
                }
                else
                {
                    neighbours[j] = null;
                }
            }
            
            points[i].GetNeighbours(neighbours);
            //if (points[i].faction != 0)
            //{
            //    foreach (Point neighbour in points[i].neighbours)
            //    {
            //        Debug.Log(neighbour);
            //    }
            //}
        }

        Debug.Log("points: " + points.Length);
    }

    private void GenerateStartPositions()
    {
        foreach(FactionSettings settings in GlobalSettings.Instance.factionSettings)
        {
            int index = GetPositionIndex(settings.StartPosition);
            points[index].faction = settings.id;
            colorMap[index] = settings.Color;
            Debug.Log("faction " + settings.id);
        }
    }

    public void Update()
    {
        Debug.Log("update");
        //for(int i = 0;i < 20*resoX;i++)
        //{
        //    points[i].Interact();
        //}
        //for (int i = 0; i < 20 * resoX; i++)
        //{
        //    points[i].Evaluate(ref colorMap);

        foreach (Point point in points)
        {
            point.Interact();
        }
        foreach (Point point in points)
        {
            point.Evaluate(ref colorMap);
        }
    }

    private int[] GetNeighbourIndices(int index)
    {
        int[] result = new int[4];
        result[0] = (index < (resoY-1)*resoX) ? index+resoX : size;
        result[1] = ((index % resoX) != resoX - 1) ? index + 1 : size;
        result[2] = (index >= resoX) ? index - resoX : size;
        result[3] = ((index % resoX) != 0) ? index + 1 : size;
        return result;
    }

    public int GetPositionIndex(Vector2Int position)
    {
        return resoX*(position.y-1) + position.x - 1;
    }
}
