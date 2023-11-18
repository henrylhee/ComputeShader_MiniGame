
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
using UnityEngine.Events;
using System.Collections;
using System.Threading.Tasks;

public class Map
{
    public FactionDataManager factionDataManager;

    private ComputeShader computeShader;
    private ComputeShader setupShader;

    public RenderTexture renderTexture;

    public ComputeBuffer factionDataBuffer;
    public ComputeBuffer pointsBuffer;
    public ComputeBuffer impactValuesBuffer;
    public ComputeBuffer pointsInjectBuffer;

    public ComputeBuffer test;

    public UnityEvent PixelsInjected;

    int CSMainKernel;
    int CSInjectPointsKernel;

    float4S[] t;

    float timeSeed = 0;

    Point[] points;
    float[] impactValues;
    int4S[] injectionPoints;

    private int size;
    private int resoX;
    private int resoY;
    private int injectionRadius;
    private int injectionReso;
    private int injectionCount;

    float brightnessInputIncrease;
    float brightnessMin;

    private int threadGroupsX;
    private int threadGroupsY;


    public void Initialize()
    {
        factionDataManager = new FactionDataManager();
        factionDataManager.Initialize();

        resoX = GameModel.Instance.resolution.width;
        resoY = GameModel.Instance.resolution.height;
        threadGroupsX = Mathf.CeilToInt(resoX / 8);
        threadGroupsY = Mathf.CeilToInt(resoY / 8);
        size = resoX * resoY;

        float injectSize = ((PlayerFactionSettings)GlobalSettings.Instance.factionSettings[1]).InjectionSize;
        injectionRadius = Mathf.RoundToInt(injectSize * resoX);
        injectionReso = injectionRadius * 2 + 1;

        brightnessInputIncrease = GlobalSettings.Instance.gameSettings.BrightnessInputIncrease;
        brightnessMin = GlobalSettings.Instance.gameSettings.BrightnessMin;

        Debug.Log("width resolution: " + resoX);
        Debug.Log("height resolution: " + resoY);
        Debug.Log("array size: " + size);

        points = new Point[size];
        impactValues = new float[size*4];
     
        for (int i = 0; i < size; i++)
        {
            points[i] = new Point(i, 0, 0, 0, GetNeighbourIndices(i));
            impactValues[i * 4 + 0] = 0f;
            impactValues[i * 4 + 1] = 0f;
            impactValues[i * 4 + 2] = 0f;
            impactValues[i * 4 + 3] = 0f;
        }

        GenerateStartPositions();

        InitializeRenderTexture();
        InitializeComputeShader();

        PixelsInjected = new UnityEvent();

        Debug.Log("points: " + points.Length);
    }

    

    public void Update()
    {
        timeSeed += Time.deltaTime;
        factionDataManager.Update();
        factionDataBuffer.SetData(factionDataManager.GetFactionData());
        ComputeShaderUpdate();
    }

    private void ComputeShaderUpdate()
    {
        //Debug.Log("--->>>> "+ factionDataManager.accumulatedStrength);
        computeShader.SetBuffer(0, "factionDataBuffer", factionDataBuffer);
        computeShader.SetFloat("timeSeed", timeSeed);
        computeShader.SetInt("hasInteracted", 0);
        computeShader.Dispatch(CSMainKernel, threadGroupsX, threadGroupsY, 1);
        computeShader.SetInt("hasInteracted", 1);
        computeShader.Dispatch(CSMainKernel, threadGroupsX, threadGroupsY, 1);
        test.GetData(t);
        //Debug.Log("-->>"+points[0].neighbourIndices.x);
        //Debug.Log(points[0].neighbourIndices.y);
        //Debug.Log(points[0].neighbourIndices.z);
        //Debug.Log(points[0].neighbourIndices.w);
        //Debug.Log("#### Time seed: ####"+t[0].x);
        //Debug.Log("#### random value: ####" + t[0].y);
        //Debug.Log("x: " + t[0].x);
        //Debug.Log("y: " + t[0].y);
        //Debug.Log("z: " + t[0].z);
        //Debug.Log("w: " + t[0].w);
    }

    private void InitializeRenderTexture()
    {
        renderTexture = new RenderTexture(resoX, resoY, 32);
        renderTexture.enableRandomWrite = true;
        renderTexture.format = RenderTextureFormat.ARGBFloat;
        renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat;
        Debug.Log("--> " + renderTexture.graphicsFormat);
        Debug.Log("--> " + renderTexture.format);
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
        CSMainKernel = computeShader.FindKernel("CSMain");
        CSInjectPointsKernel = computeShader.FindKernel("CSInjectPoints");

        pointsBuffer = new ComputeBuffer(size, sizeof(int) * 8);
        pointsBuffer.SetData(points);

        impactValuesBuffer = new ComputeBuffer(size*4, sizeof(float));
        impactValuesBuffer.SetData(impactValues);

        factionDataBuffer = new ComputeBuffer(factionDataManager.factionCount, sizeof(float) * 8);
        factionDataBuffer.SetData(factionDataManager.GetFactionData());

        pointsInjectBuffer = new ComputeBuffer(Mathf.CeilToInt((injectionReso * injectionReso) / 2), sizeof(int)*4);


        test = new ComputeBuffer(1, sizeof(float)*4);
        t = new float4S[1];
        t[0] = new float4S(0, 0, 0, 0);
        test.SetData(t);


        
        computeShader.SetBuffer(CSInjectPointsKernel, "pointsInjectBuffer", pointsInjectBuffer);
        computeShader.SetBuffer(CSInjectPointsKernel, "test", test);
        computeShader.SetInt("injectionReso", injectionReso);


        computeShader.SetInt("resoX", resoX);
        computeShader.SetInt("resoY", resoY);
        computeShader.SetInt("threadCountX", threadGroupsX * 8);
        computeShader.SetInt("threadCountY", threadGroupsY * 8);
        computeShader.SetInt("indexCount", size);

        computeShader.SetFloat("brightnessMin", brightnessMin);
        computeShader.SetFloat("brightnessInputIncrease", brightnessInputIncrease);

        //computeShader.SetFloat("enemyStrength", factionScaling.accumulatedStrength);

        computeShader.SetTexture(CSMainKernel, "colorTexture", renderTexture);
        computeShader.SetTexture(CSInjectPointsKernel, "colorTexture", renderTexture);

        computeShader.SetBuffer(CSMainKernel, "factionDataBuffer", factionDataBuffer);
        computeShader.SetBuffer(CSInjectPointsKernel, "factionDataBuffer", factionDataBuffer);
        computeShader.SetBuffer(CSMainKernel, "pointsBuffer", pointsBuffer);
        computeShader.SetBuffer(CSInjectPointsKernel, "pointsBuffer", pointsBuffer);
        computeShader.SetBuffer(CSMainKernel, "impactValuesBuffer", impactValuesBuffer);
        computeShader.SetBuffer(CSMainKernel, "test", test);
    }

    private void GenerateStartPositions()
    {
        List<FactionSettings> settings = GlobalSettings.Instance.factionSettings;
        for (int i = 1; i < factionDataManager.factionCount; i++)
        {
            int index = GetPositionIndex(settings[i].StartPosition);
            points[index].faction = settings[i].id;
            points[index].isActive = 1;
            Debug.Log("faction " + settings[i].id);
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

    public void InjectPixels(int positionX, int positionY)
    {

        List<int[]> pointIndices = new List<int[]>();
        pointIndices = Rasterization.GetPointsInCircle(positionX, positionY, injectionRadius);
        injectionCount = Mathf.CeilToInt(pointIndices.Count / 2);

        computeShader.SetInt("injectionCount", injectionCount);

        injectionPoints = new int4S[Mathf.CeilToInt((injectionReso*injectionReso)/2)];
        int index = 0;
        for (int  i = 0; i < pointIndices.Count; i++)
        {
            if (i%2 == 0)
            {
                injectionPoints[index].x = pointIndices[i][0];
                injectionPoints[index].y = pointIndices[i][1];
            }
            else
            {
                injectionPoints[index].z = pointIndices[i][0];
                injectionPoints[index].w = pointIndices[i][1];

                index++;
            }           
        }
        if (pointIndices.Count % 2 != 0)
        {
            injectionPoints[injectionCount-1].z = -1;
            injectionPoints[injectionCount-1].w = -1;
        }

        Debug.Log("Mouse clicked at: " + positionX + ", " + positionY);


        pointsInjectBuffer.SetData(injectionPoints);

        computeShader.Dispatch(CSInjectPointsKernel, Mathf.CeilToInt(injectionReso/8), Mathf.CeilToInt(injectionReso/8), 1);

        Debug.Log(CSMainKernel);
        Debug.Log(CSInjectPointsKernel);
        
        Debug.Log("amount of injected points: " + pointIndices.Count);
        Debug.Log("buffer length: " + injectionPoints.Length);
        Debug.Log("injection resolution: " + injectionReso);
        test.GetData(t);
        //Debug.Log(t[0].x);
        //Debug.Log(t[0].y);
        //Debug.Log(t[0].z);
        //Debug.Log(t[0].w);
        factionDataManager.PixelsInjected();
        PixelsInjected.Invoke();
    }

    public void ReleaseComputeBuffer()
    {
        factionDataBuffer.Release();
        pointsBuffer.Release();
        impactValuesBuffer.Release();
        test.Release();
        pointsInjectBuffer.Release();
    }

    private void GetStartIndices(int x, int y )
    {

    }

    private int[][] Get8And1Neighbours(int x, int y)
    {
        int[][] result = new int[8][];

        int x0 = x-1;
        int y0 = y-1;
        for (int yIndex = y0; yIndex < y0+3; yIndex++)
        {
            for (int xIndex = x0; xIndex < x0+3; xIndex++)
            {

            }
        }

        return result;
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

public struct int4S
{
    public int x;
    public int y;
    public int z;
    public int w;
    public int4S(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}