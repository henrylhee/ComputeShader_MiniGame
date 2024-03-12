using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
