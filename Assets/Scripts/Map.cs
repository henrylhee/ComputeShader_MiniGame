
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


    public Color[] colorMap { get; private set; }
    public Dictionary<int,FractionSettings> fractionSettings { get; private set; }
    public List<Point> activePoints { get; private set; }
    private Dictionary<int,int> pointsToRemoveIndices;
    private List<Point> pointsToAdd;

    private bool4[] validDirMap;

    public int[] ids { get; private set; }
    public bool[] activityMap { get; private set; }

    private int size;
    private Resolution resolution;

    readonly int[] helperX = new int[4] {0,1,0,-1};
    readonly int[] helperY = new int[4] {1,0,-1,0};


    public void Initialize(Dictionary<int, FractionSettings> fractionSettings)
    {
        resolution = GameModel.Instance.resolution;
        size = resolution.width * resolution.height;

        activePoints = new List<Point>();
        pointsToAdd = new List<Point>();

        Color initColor = new Color(0, 0, 0, 0);
        int emptyId = 0;
        bool[] initCanExpand = new bool[4] { true, true, true, true };

        activityMap = new bool[size];
        colorMap = new Color[size];
        ids = new int[size];
        for (int i = 0; i < size; i++)
        {
            colorMap[i] = initColor;
            ids[i] = emptyId;
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
            int[] neighbourData = GetNeighbourIds(position);

            for (int direction = 0; direction < 4; direction++)
            {
                if (neighbourData[direction] != size)
                {
                    Vector2Int newPos = CalculateNewPosition(position, direction);
                    int newIndex = GetPositionIndex(newPos);
                    if (neighbourData[direction] == 0)
                    {
                        if (Random.Range(0f, 1f) < fractionSettings[fractionId].ExpansionRate)
                        {
                            AddPoint(fractionId, newPos, newIndex);
                        }
                    }
                    else if (neighbourData[direction] != fractionId)
                    {
                        if (Random.Range(0f, 1f) < fractionSettings[fractionId].ConquerRate)
                        {
                            ConquerPoint(fractionId, newPos, newIndex, direction);
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
                activityMap[activePoints[i].index] = false;
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
        for (int i = 0;i < direction;i++)
        {
            if(i == direction%4)
        }
    }

    private void AddPoint(int id, Vector2Int position, int index)
    {
        colorMap[index] = fractionSettings[id].Color;
        pointsToAdd.Add(new Point(position, id, index));
        ids[index] = id;
        activityMap[index] = true; 
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
        result[0] = (index < (resolution.height-1)*resolution.width) ? index+resolution.width : size;
        result[1] = ((index % resolution.width) != resolution.width - 1) ? index + 1 : size;
        result[2] = (index >= resolution.width) ? index - resolution.width : size;
        result[3] = ((index % resolution.width) != 0) ? index + 1 : size;
        return result;
    }

    private void InitializeValidDirMap()
    {
        validDirMap = new bool4[size];
        bool4 initValue = new bool4(true, true, true, true);

        validDirMap[0] = new bool4(true, true, false, false);

        for (int i = 1; i < size; i++)
        {
            if (i == resolution.width - 1) { new bool4(true, false, false, true); }
            else if (i == (resolution.height - 1) * resolution.width) { validDirMap[i] = new bool4(false, true, true, false); }
            else if(i == size - 1) { validDirMap[i] = new bool4(false, false, true, true); }
            else if(i >= 1 && i < resolution.width - 1) { validDirMap[i] = new bool4(true, true, false, true); }
            else if (i >= (resolution.height - 1) * resolution.width + 1 && i < size - 1) { validDirMap[i] = new bool4(false, true, true, true); }
            else if (i % resolution.width == 0) { validDirMap[i] = new bool4(true, true, true, false); }
            else if (i % resolution.width == resolution.width - 1) { validDirMap[i] = new bool4(true, false, true, true); }
            else { validDirMap[i] = initValue; }
        }
    }

    public int GetPositionIndex(Vector2Int position)
    {
        return resolution.width*(position.y-1) + position.x - 1;
    }

    public Vector2Int GetIndexPosition(int index)
    {
        int y = (int)(index / resolution.width) + 1;
        return new Vector2Int(index-(y-1)*resolution.width+1, y);
    }

    private bool IsPositionValid(Vector2Int position)
    {
        if (position.x < 1) { return false; }
        else if (position.y < 1) {  return false; }
        else if (position.x > resolution.width) { return false; }
        else if (position.y > resolution.height) {  return false; }
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
