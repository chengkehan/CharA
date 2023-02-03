#ifndef __GET_LIGHT_DATA__
#define __GET_LIGHT_DATA__

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void GetMainLight_float(in float3 WorldPos, out float3 Direction)
{
	// grab the shadow coordinates
	#if SHADOWS_SCREEN
		half4 shadowCoord = ComputeScreenPos(TransformWorldToHClip(WorldPos));
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
	#endif 

	// grab the main light
	#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
		Light light = GetMainLight(shadowCoord);
	#else
		Light light = GetMainLight();
	#endif

	Direction = light.direction;
}

float3 GetMainLightDir(float3 posWS)
{
	float3 dir = 0;
	GetMainLight_float(posWS, dir);
	return dir;
}

#endif