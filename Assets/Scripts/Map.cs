
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using System;
using UnityEditor.Rendering.LookDev;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Map
{
    private ComputeShader computeShader;
    private ComputeShader setupShader;
    public RenderTexture renderTexture;

    ComputeBuffer factionDataBuffer;
    ComputeBuffer pointsBuffer;
    ComputeBuffer impactValuesBuffer;

    Point[] points;
    float[] impactValues;
    FactionData[] factionData;
    int factionCount;

    private int size;
    private int resoX;
    private int resoY;

    private int threadGroupsX;
    private int threadGroupsY;




    public void Initialize()
    {
        resoX = GameModel.Instance.resolution.width;
        resoY = GameModel.Instance.resolution.height;
        threadGroupsX = Mathf.CeilToInt(resoX / 8);
        threadGroupsY = Mathf.CeilToInt(resoY / 8);
        size = resoX * resoY;

        Debug.Log("width resolution: " + resoX);
        Debug.Log("height resolution: " + resoY);
        Debug.Log("array size: " + size);


        factionCount = GlobalSettings.Instance.factionSettings.Count;
        factionData = new FactionData[factionCount];
        factionData[0] = new FactionData();
        for (int i = 1; i < factionCount; i++)
        {
            FactionSettings factionSettings = GlobalSettings.Instance.factionSettings[i];
            factionData[i] = new FactionData();
            factionData[i].conquerRate = factionSettings.ConquerRate;
            factionData[i].conquerStrength = factionSettings.ConquerStrength;
            factionData[i].expansionRate = factionSettings.ExpansionRate;
            factionData[i].expansionStrength = factionSettings.ExpansionStrength;
            factionData[i].color = factionSettings.Color;
        }

        points = new Point[size];
        impactValues = new float[size*4];

       
        for (int i = 0; i < size; i++)
        {
            points[i] = new Point(i, 0, 0, GetNeighbourIndices(i));
            impactValues[i * 4 + 0] = 0f;
            impactValues[i * 4 + 1] = 0f;
            impactValues[i * 4 + 2] = 0f;
            impactValues[i * 4 + 3] = 0f;
        }

        GenerateStartPositions();

        InitializeRenderTexture();
        InitializeComputeShader();

        Debug.Log("points: " + points.Length);
    }
    public void Update()
    {
        ComputeShaderUpdate();
    }

    private void ComputeShaderUpdate()
    {
        computeShader.SetInt("hasInteracted", 0);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        computeShader.SetInt("hasInteracted", 1);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
    }


    private void InitializeRenderTexture()
    {
        renderTexture = new RenderTexture(resoX, resoY, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        ComputeShader setupShader = Resources.Load<ComputeShader>("Shader/setupShader");
        setupShader.SetTexture(0, "result", renderTexture);
        setupShader.Dispatch(0,threadGroupsX,threadGroupsY,1);
    }

    private void InitializeComputeShader()
    {
        computeShader = Resources.Load<ComputeShader>("Shader/Interact");

        pointsBuffer = new ComputeBuffer(size, sizeof(int) * 8);
        pointsBuffer.SetData(points);

        impactValuesBuffer = new ComputeBuffer(size*4, sizeof(float));
        impactValuesBuffer.SetData(impactValues);

        factionDataBuffer = new ComputeBuffer(factionCount, sizeof(float) * 8);
        factionDataBuffer.SetData(factionData);

        computeShader.SetInt("ResoX", resoX);
        computeShader.SetInt("ResoY", resoY);
        computeShader.SetInt("threadCountX", threadGroupsX * 8);
        computeShader.SetInt("threadCountY", threadGroupsY * 8);
        computeShader.SetInt("indexCount", size);
        computeShader.SetTexture(0, "colorTexture", renderTexture);

        computeShader.SetBuffer(0, "factionDataBuffer", factionDataBuffer);
        computeShader.SetBuffer(0, "pointsBuffer", pointsBuffer);
        computeShader.SetBuffer(0, "impactValuesBuffer", impactValuesBuffer);
    }

    private void GenerateStartPositions()
    {
        for(int i = 1; i < factionCount; i++)
        {
            FactionSettings settings = GlobalSettings.Instance.factionSettings[i];
            int index = GetPositionIndex(settings.StartPosition);
            points[index].faction = settings.id;
            points[index].isActive = 1;
            Debug.Log("faction " + settings.id);
        }
    }


    private int4 GetNeighbourIndices(int index)
    {
        int4 result = new int4();
        result[0] = (index < (resoY-1)*resoX) ? index+resoX : size;
        result[1] = ((index % resoX) != resoX - 1) ? index + 1 : size;
        result[2] = (index >= resoX) ? index - resoX : size;
        result[3] = ((index % resoX) != 0) ? index - 1 : size;
        return result;
    }

    public int GetPositionIndex(Vector2Int position)
    {
        return resoX*(position.y-1) + position.x - 1;
    }

    

    private int CalculateWorkGroups(int reso, int numThreads)
    {
        return Mathf.CeilToInt(reso / numThreads);
    }
}

