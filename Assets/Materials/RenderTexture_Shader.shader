Shader "Unlit/RenderTexture_Shader"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "red" {}
    }
    
    Category{
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off
        SubShader{
            Pass{
                SetTexture[_MainTex] {}
            }
        }
    }
}
