
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


    public static Color[] colorMap { get; private set; }
    public Dictionary<int,FractionSettings> factionSettings { get; private set; }
    private Point[] points;

    public int[] ids { get; private set; }

    private int size;
    private int resoX;
    private int resoY;

    readonly int[] helperX = new int[4] {0,1,0,-1};
    readonly int[] helperY = new int[4] {1,0,-1,0};


    public void Initialize(Dictionary<int, FractionSettings> fractionSettings)
    {
        resoX = GameModel.Instance.resolution.width;
        resoY = GameModel.Instance.resolution.height;
        size = resoX * resoY;

        points = new Point[size];

        Color initColor = new Color(0, 0, 0, 0);
        int emptyId = 0;
        bool[] initCanExpand = new bool[4] { true, true, true, true };

        colorMap = new Color[size];
        ids = new int[size];
        for (int i = 0; i < size; i++)
        {
            colorMap[i] = initColor;
            ids[i] = emptyId;
            points[i] = new Point();
        }

        InitializeValidDirMap();
        
        this.fractionSettings = fractionSettings;

        GenerateStartPositions();
    }

    private void GenerateStartPositions()
    {
        for(int i = 1; i <= fractionSettings.Count; i++)
        {
            AddPoint(i, fractionSettings[i].StartPosition, GetPositionIndex(fractionSettings[i].StartPosition));
        }
        foreach(Point point in pointsToAdd)
        {
            activePoints.Add(point);
        }
    }

    public void Update()
    {
        Debug.Log("active point count: " + activePoints.Count);

        pointsToAdd = new List<Point>();
        foreach (Point point in activePoints)
        {
            Vector2Int position = point.position;
            int index = point.index;
            int fractionId = point.fractionId;
            int[] neighbourIds = GetNeighbourIds(position);

            for (int direction = 0; direction < 4; direction++)
            {
                if (neighbourIds[direction] != size)
                {
                    Vector2Int newPos = CalculateNewPosition(position, direction);
                    int newIndex = GetPositionIndex(newPos);
                    if (neighbourIds[direction] == 0)
                    {
                        if (Random.Range(0f, 1f) < fractionSettings[fractionId].ExpansionRate)
                        {
                            AddPoint(fractionId, newPos, newIndex);
                        }
                    }
                    else if (neighbourIds[direction] != fractionId)
                    {
                        if (Random.Range(0f, 1f) < fractionSettings[fractionId].ConquerRate)
                        {
                            ConquerPoint(fractionId, newPos, newIndex, (direction+2)%4);
                        }
                    }
                }
            }
        }

        activePoints.AddRange(pointsToAdd);

        int limit = activePoints.Count;
        for (int i = 0; i < limit; i++)
        {
            //overwrite conquered id
            activePoints[i].fractionId = ids[activePoints[i].index];

            if (IsInactive(activePoints[i].index, activePoints[i].fractionId))
            {
                activePoints.RemoveAt(i);
                i--;
                limit--;
            }
        }
    }

    private void ConquerPoint(int id, Vector2Int position, int index, int direction)
    {
        
        colorMap[index] = fractionSettings[id].Color;
        ids[index] = id;
        int[] neighbourIndices = GetNeighbourIndices(index);
        for (int i = 0; i < 4; i++)
        {
            int neighbourIndex = neighbourIndices[i];
            if(i != direction && ids[neighbourIndex] != 0 && neighbourIndex != size)
            {
                pointsToAdd.Add(new Point(position, id, index));
            }
        }
    }

    private void AddPoint(int id, Vector2Int position, int index)
    {
        colorMap[index] = fractionSettings[id].Color;
        pointsToAdd.Add(new Point(position, id, index));
        ids[index] = id;
    }

    private Vector2Int CalculateNewPosition(Vector2Int oldPos, int dirIndex)
    {
        Vector2Int newPos;

        switch (dirIndex)
        {
            case 0:
                newPos = new Vector2Int(oldPos.x, oldPos.y + 1);
                break;
            case 1:
                newPos = new Vector2Int(oldPos.x + 1, oldPos.y);
                break;
            case 2:
                newPos = new Vector2Int(oldPos.x, oldPos.y - 1);
                break;
            case 3:
                newPos = new Vector2Int(oldPos.x - 1, oldPos.y);
                break;
            default:
                newPos = new Vector2Int(0, 0);
                break;
        }
        return newPos;
    }

    private void UpdateValidDir(int index, int direction, bool value)
    {
        validDirMap[index][direction] = value;
    }

    private int[] GetNeighbourIds(Vector2Int position)
    {
        int[] neighbourData = new int[4];

        int neighbourIndex;
        for (int i = 0;i < 4; i++)
        {
            Vector2Int newPos = CalculateNewPosition(position, i);
            if (IsPositionValid(newPos))
            {
                neighbourIndex = GetPositionIndex(newPos);
                neighbourData[i] = ids[neighbourIndex];
            }
            else
            {
                neighbourData[i] = size;
            }
        }
        return neighbourData;
    }

    private Vector2Int[] GetNeighbourPositions(Vector2Int position)
    {
        Vector2Int[] neighbourPositions = new Vector2Int[4];

        for (int i = 0; i < 4; i++)
        {
            neighbourPositions[i] = CalculateNewPosition(position, i);
        }
        return neighbourPositions;
    }

    private bool IsInactive(int index, int id)
    {
        int[] neighbourIndices = GetNeighbourIndices(index); ;
        for (int i = 0; i < 4; i++)
        {
            if (neighbourIndices[i] != size)
            {
                if (ids[neighbourIndices[i]] == 0) { return false; }
                if (ids[neighbourIndices[i]] != id) { return false; }
            }
        }
        return true;
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

    private void InitializeValidDirMap()
    {
        validDirMap = new bool4[size];
        bool4 initValue = new bool4(true, true, true, true);

        validDirMap[0] = new bool4(true, true, false, false);

        for (int i = 1; i < size; i++)
        {
            if (i == resoX - 1) { new bool4(true, false, false, true); }
            else if (i == (resoY - 1) * resoX) { validDirMap[i] = new bool4(false, true, true, false); }
            else if(i == size - 1) { validDirMap[i] = new bool4(false, false, true, true); }
            else if(i >= 1 && i < resoX - 1) { validDirMap[i] = new bool4(true, true, false, true); }
            else if (i >= (resoY - 1) * resoX + 1 && i < size - 1) { validDirMap[i] = new bool4(false, true, true, true); }
            else if (i % resoX == 0) { validDirMap[i] = new bool4(true, true, true, false); }
            else if (i % resoX == resoX - 1) { validDirMap[i] = new bool4(true, false, true, true); }
            else { validDirMap[i] = initValue; }
        }
    }

    public int GetPositionIndex(Vector2Int position)
    {
        return resoX*(position.y-1) + position.x - 1;
    }

    public Vector2Int GetIndexPosition(int index)
    {
        int y = (int)(index / resoX) + 1;
        return new Vector2Int(index-(y-1)*resoX+1, y);
    }

    private bool IsPositionValid(Vector2Int position)
    {
        if (position.x < 1) { return false; }
        else if (position.y < 1) {  return false; }
        else if (position.x > resoX) { return false; }
        else if (position.y > resoY) {  return false; }
        return true;
    }
}

public class PointReferenceArray
{
    public Point[] neighbors;
    public PointReferenceArray()
    {
        neighbors = new Point[4];
    }
}
