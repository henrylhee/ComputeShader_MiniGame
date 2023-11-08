
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

    int kernelHandleInteract;
    int kernelHandleEvaluate;

    Point[] points;
    float4[] impactValues;
    FactionData[] factionData;
    int factionCount;

    public int[] ids { get; private set; }

    private int size;
    private int resoX;
    private int resoY;

    private int threadGroupsX;
    private int threadGroupsY;


    readonly int[] helperX = new int[4] {0,1,0,-1};
    readonly int[] helperY = new int[4] {1,0,-1,0};




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

        InitializeRenderTexture();

        factionCount = GlobalSettings.Instance.factionSettings.Count;
        factionData = new FactionData[factionCount];
        for (int i = 0; i < factionCount; i++)
        {
            FactionSettings factionSettings = GlobalSettings.Instance.factionSettings[i];
            factionData[i].conquerRate = factionSettings.ConquerRate;
            factionData[i].conquerStrength = factionSettings.ConquerStrength;
            factionData[i].expansionRate = factionSettings.ExpansionRate;
            factionData[i].expansionStrength = factionSettings.ExpansionStrength;
            factionData[i].color = factionSettings.Color;
        }

        points = new Point[size];
        impactValues = new float4[size];

        int emptyId = 0;

        ids = new int[size];
        
        for (int i = 0; i < size; i++)
        {
            ids[i] = emptyId;
            points[i] = new Point(i, 0, false, GetNeighbourIndices(i));
            impactValues[i] = new float4(0,0,0,0);
        }

        GenerateStartPositions();

        Debug.Log("points: " + points.Length);
    }

    private void ComputeShaderInteract()
    {
        computeShader.SetInt("ResoX", resoX);
        computeShader.SetInt("ResoY", resoY);
        computeShader.SetInt("threadCountX", threadGroupsX*8);
        computeShader.SetInt("threadCountY", threadGroupsY*8);
        computeShader.SetInt("indexCount", size);
        computeShader.SetTexture(kernelHandleInteract, "colorTexture", renderTexture);
        computeShader.SetBuffer(kernelHandleInteract, "factionDataBuffer", factionDataBuffer);
        computeShader.SetBuffer(kernelHandleInteract, "pointsBuffer", pointsBuffer);
        computeShader.SetBuffer(kernelHandleInteract, "impactValuesbuffer", impactValuesBuffer);

        computeShader.Dispatch(kernelHandleInteract, threadGroupsX, threadGroupsY, 1);

        
    }

    private void ComputeShaderEvaluate()
    {
        computeShader.SetInt("ResoX", resoX);
        computeShader.SetInt("ResoY", resoY);
        computeShader.SetInt("threadCountX", threadGroupsX * 8);
        computeShader.SetInt("threadCountY", threadGroupsY * 8);
        computeShader.SetInt("indexCount", size);
        computeShader.SetTexture(kernelHandleInteract, "colorTexture", renderTexture);
        computeShader.SetBuffer(kernelHandleInteract, "factionDataBuffer", factionDataBuffer);
        computeShader.SetBuffer(kernelHandleInteract, "pointsBuffer", pointsBuffer);
        computeShader.SetBuffer(kernelHandleInteract, "impactValuesbuffer", impactValuesBuffer);

        computeShader.Dispatch(kernelHandleInteract, threadGroupsX, threadGroupsY, 1);


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
        computeShader = Resources.Load<ComputeShader>("Shader/ComputeShader");
        kernelHandleInteract = computeShader.FindKernel("Interact");
        kernelHandleEvaluate = computeShader.FindKernel("Evaluate");
    }

    private void InitializeBuffers()
    {
        pointsBuffer = new ComputeBuffer(size, sizeof(int)*6+sizeof(bool));
        pointsBuffer.SetData(points);

        impactValuesBuffer = new ComputeBuffer(size, sizeof(float)*4);
        impactValuesBuffer.SetData(impactValues);

        factionDataBuffer = new ComputeBuffer(factionCount, sizeof(float) * 8);
        factionDataBuffer.SetData(factionData);
    }

    private void GenerateStartPositions()
    {
        foreach(FactionSettings settings in GlobalSettings.Instance.factionSettings)
        {
            int index = GetPositionIndex(settings.StartPosition);
            points[index].faction = settings.id;
            points[index].isActive = true;
            Debug.Log("faction " + settings.id);
        }
    }

    public void Update()
    {
        //Debug.Log("update");
        //for(int i = 0;i < 20*resoX;i++)
        //{
        //    points[i].Interact();
        //}
        //for (int i = 0; i < 20 * resoX; i++)
        //{
        //    points[i].Evaluate(ref colorMap);

        //foreach (Point point in points)
        //{
        //    point.Interact();
        //}
        //foreach (Point point in points)
        //{
        //    point.Evaluate(ref colorMap);
        //}
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

