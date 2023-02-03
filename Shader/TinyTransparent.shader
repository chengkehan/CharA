Shader "Custom/TinyTransparent"
{
    Properties
    {
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        _Intensity("Intensity", Range(0, 2)) = 1
        _AlphaOffset("Alpha Offset", Range(-1, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define IS_TRANSPARENT 1

            #include "Assets/GameRes/Shader/TinyCommon.hlsl"

            ENDHLSL
        }
    }
}
