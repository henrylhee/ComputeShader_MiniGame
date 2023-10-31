
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using System;
using UnityEditor.Rendering.LookDev;

public class Map
{
    //problems to tackle: order of executions in pointlist???


    public Color[] colorMap { get; private set; }
    public Dictionary<int,FractionSettings> fractionSettings { get; private set; }
    public List<Point> inactivePoints { get; private set; }
    public List<Point> activePoints { get; private set; }
    private Dictionary<int,int> pointsToRemoveIndices;
    private List<Point> pointsToAdd;

    public bool4[] validDirMap { get; private set; }


    public int[] ids { get; private set; }

    // points zentral here. with id. id-settings dictionary. events

    private int size;
    private Resolution resolution;

    readonly int[] helperX = new int[4] {0,1,0,-1};
    readonly int[] helperY = new int[4] {1,0,-1,0};


    public void Initialize(Dictionary<int, FractionSettings> fractionSettings)
    {
        resolution = GameModel.Instance.resolution;
        size = resolution.width * resolution.height;

        activePoints = new List<Point>();
        inactivePoints = new List<Point>();
        pointsToAdd = new List<Point>();

        Color initColor = new Color(0, 0, 0, 0);
        int idEmpty = 0;
        bool[] initCanExpand = new bool[4] { true, true, true, true };
        

        colorMap = new Color[size];
        ids = new int[size];
        for (int i = 0; i < size; i++)
        {
            colorMap[i] = initColor;
            ids[i] = idEmpty;
        }

        InitializeValidDirMap();
        
        this.fractionSettings = fractionSettings;

        GenerateStartPositions();
    }

    private void GenerateStartPositions()
    {
        for(int i = 1; i <= fractionSettings.Count; i++)
        {
            AddPoint(i, fractionSettings[i].StartPosition);
        }
        foreach(Point point in pointsToAdd)
        {
            activePoints.Add(point);
        }
    }

    public void Update()
    {
        //Debug.Log("active point count: " + activePoints.Count);
        //Debug.Log("inactive point count: " + inactivePoints.Count);

        pointsToAdd = new List<Point>();
        foreach (Point point in activePoints)
        {
            Vector2Int position = point.position;
            int index = GetPositionIndex(position);
            bool4 canExpand = validDirMap[index];
            int fractionId = point.fractionId;

            for (int direction = 0; direction < 4; direction++)
            {
                Vector2Int newPos = CalculateNewPosition(position, direction);
                int newIndex = GetPositionIndex(newPos);

                if (canExpand[direction])
                {
                    if(Random.Range(0f, 1f) < fractionSettings[fractionId].ExpansionRate)
                    {
                        AddPoint(fractionId, newPos);
                    }
                }
                else if (ids[newIndex] != fractionId)
                {
                    if (Random.Range(0f,1f) < fractionSettings[fractionId].ConquerRate)
                    {
                        ConquerPoint(fractionId, newPos);
                    }
                }
            }
        }

        activePoints.AddRange(pointsToAdd);

        int limit = activePoints.Count;
        for (int i = 0; i < limit; i++)
        {
            if (IsInactive(GetPositionIndex(activePoints[i].position))) 
            {
                activePoints.RemoveAt(i);
                i--;
                limit--;
            }
        }
    }

    private void ConquerPoint(int fractionId, Vector2Int newPos)
    {
        ;
    }

    public void UpdateState()
    {
        
    }

    private void AddPoint(int id, Vector2Int position)
    {
        int index = GetPositionIndex(position);
        ids[index] = id;
        this.colorMap[index] = fractionSettings[id].Color;
        Point point = new Point(position, id);
        if (IsInactive(index))
        {
            inactivePoints.Add(point);
        }
        else
        {
            pointsToAdd.Add(point);
        }

        Vector2Int checkPos;
        int checkIndex;
        
        for (int direction = 0; direction < 4; direction++)
        {
            checkPos.x = position.x + helperX[direction];
            checkPos.y = position.y + helperY[direction];

            if(checkPos.x <= resolution.width && checkPos.x > 0 && checkPos.y <= resolution.height && checkPos.y > 0)
            {
                checkIndex = GetPositionIndex(checkPos);
                UpdateValidDir(checkIndex,((direction + 2) % 4),false);
            }
        }
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

    private bool IsInactive(int index)
    {
        bool4 canExpand = validDirMap[index];
        if (canExpand[0] == true) { return false; }
        else if (canExpand[1] == true) { return false; }
        else if (canExpand[2] == true) { return false; }
        else if (canExpand[3] == true) { return false; }
        return true;
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
