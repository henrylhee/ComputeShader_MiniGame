
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

    public ComputeBuffer factionDataBuffer;
    public ComputeBuffer pointsBuffer;
    public ComputeBuffer impactValuesBuffer;

    public ComputeBuffer test;
    public ComputeBuffer timeSeedBuffer;

    float4S[] t;

    float timeSeed = 0;

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

        InitializeFactionData();

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
        timeSeed += Time.deltaTime;
        ComputeShaderUpdate();
    }

    private void ComputeShaderUpdate()
    {
        computeShader.SetFloat("timeSeed", timeSeed);
        computeShader.SetInt("hasInteracted", 0);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        computeShader.SetInt("hasInteracted", 1);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        test.GetData(t);
        //Debug.Log("-->>"+points[0].neighbourIndices.x);
        //Debug.Log(points[0].neighbourIndices.y);
        //Debug.Log(points[0].neighbourIndices.z);
        //Debug.Log(points[0].neighbourIndices.w);
        //Debug.Log("#### Time seed: ####"+t[0].x);
        //Debug.Log("#### random value: ####" + t[0].y);
        //Debug.Log(t[0].z);
        //Debug.Log(t[0].w);
    }

    private void InitializeRenderTexture()
    {
        renderTexture = new RenderTexture(resoX, resoY, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        Debug.Log("Initialize rendertexture");
        ComputeShader setupShader = Resources.Load<ComputeShader>("Shader/SetupShader");
        setupShader.SetInt("resoX", resoX);
        setupShader.SetInt("resoY", resoY);
        setupShader.SetVector("initColor", new Color(0,0,0,1));
        setupShader.SetTexture(0, "result", renderTexture);
        setupShader.Dispatch(0,threadGroupsX,threadGroupsY,1);
    }

    private void InitializeComputeShader()
    {
        Debug.Log("Initialize compute shader");
        computeShader = Resources.Load<ComputeShader>("Shader/Interact");

        pointsBuffer = new ComputeBuffer(size, sizeof(int) * 8);
        pointsBuffer.SetData(points);

        impactValuesBuffer = new ComputeBuffer(size*4, sizeof(float));
        impactValuesBuffer.SetData(impactValues);

        factionDataBuffer = new ComputeBuffer(factionCount, sizeof(float) * 8);
        factionDataBuffer.SetData(factionData);

        test = new ComputeBuffer(1, sizeof(float)*4);
        t = new float4S[1];
        t[0] = new float4S(0, 0, 0, 0);
        test.SetData(t);

        
        computeShader.SetInt("resoX", resoX);
        computeShader.SetInt("resoY", resoY);
        computeShader.SetInt("threadCountX", threadGroupsX * 8);
        computeShader.SetInt("threadCountY", threadGroupsY * 8);
        computeShader.SetInt("indexCount", size);
        computeShader.SetTexture(0, "colorTexture", renderTexture);

        computeShader.SetBuffer(0, "factionDataBuffer", factionDataBuffer);
        computeShader.SetBuffer(0, "pointsBuffer", pointsBuffer);
        computeShader.SetBuffer(0, "impactValuesBuffer", impactValuesBuffer);
        computeShader.SetBuffer(0, "test", test);
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

    public void UpdateSettings()
    {
        InitializeFactionData();
        factionDataBuffer.SetData(factionData);
        computeShader.SetBuffer(0, "factionDataBuffer", factionDataBuffer);
    }

    private void InitializeFactionData()
    {
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
    }

    public void InjectPixels(int positionX, int positionY)
    {
        
    }
}


public struct float4S
{
    public float x;
    public float y;
    public float z;
    public float w;
    public float4S(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}