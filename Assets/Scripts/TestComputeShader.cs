using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;

public class TestComputeShader : MonoBehaviour
{
    private ComputeShader computeShader;
    public RenderTexture rendertexture;

    public int resoX;
    public int resoY;


    private void Start()
    {
        computeShader = Resources.Load<ComputeShader>("Shader/ComputeShader");

        resoX = Screen.currentResolution.width;
        resoY = Screen.currentResolution.height;

        rendertexture = new RenderTexture(resoX, resoY, 24);
        rendertexture.enableRandomWrite = true;
        rendertexture.Create();

        computeShader.SetFloat("ResoX", rendertexture.width);
        computeShader.SetFloat("ResoY", rendertexture.height);
        computeShader.SetTexture(0, "Result", rendertexture);



        ComputeBuffer factionDataBuffer = new ComputeBuffer(resoX*resoY, sizeof(float)*8);





        computeShader.Dispatch(0, rendertexture.width / 8, rendertexture.height / 8, 1);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {


        Graphics.Blit(rendertexture, destination);
    }

    private int CalculateWorkGroups(int reso, int numThreads)
    {
        return Mathf.CeilToInt(reso / (numThreads * 32)) * 32;
    }



}



