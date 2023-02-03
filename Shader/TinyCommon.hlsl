#ifndef __TINY_COMMON__
#define __TINY_COMMON__

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Assets/GameRes/Shader/GetLightData.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
    float3 normal : NORMAL;
};

struct Varyings
{
    float2 uv           : TEXCOORD0;
    float4 positionHCS  : SV_POSITION;
    float lambert : TEXCOORD1;
};
            
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half _Intensity;
half _AlphaOffset;
CBUFFER_END

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.uv = TRANSFORM_TEX(IN.uv.xy, _BaseMap);

    float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
    float3 lightDirWS = GetMainLightDir(posWS);
    float3 normalWS = TransformObjectToWorldDir(IN.normal);
    float lambert = max(dot(lightDirWS, normalWS), 0);
    lambert = pow(lambert, 0.5);
    lambert = saturate(lambert + 0.25);
    OUT.lambert = lambert;

    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
    half4 c = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv.xy) * _BaseColor;
    c.rgb *= _Intensity * IN.lambert;
#ifdef IS_TRANSPARENT
    c.a += _AlphaOffset;
#endif
    return c;
}

#endif